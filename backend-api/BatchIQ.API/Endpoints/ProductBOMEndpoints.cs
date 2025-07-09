using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class ProductBOMEndpoints
{
    public static void MapProductBOMEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/product-bom").WithTags("Product BOM");

        // Get all BOM entries
        group.MapGet("/", GetAllBOMs).RequireAuthorization("BOMView");
        
        // Get BOM by parent product
        group.MapGet("/product/{productId:int}", GetBOMByProduct).RequireAuthorization("BOMView");
        
        // Get specific BOM entry
        group.MapGet("/{id:int}", GetBOMById).RequireAuthorization("BOMView");
        
        // Create new BOM entry
        group.MapPost("/", CreateBOM).RequireAuthorization("BOMCreate");
        
        // Update BOM entry
        group.MapPut("/{id:int}", UpdateBOM).RequireAuthorization("BOMEdit");
        
        // Delete BOM entry
        group.MapDelete("/{id:int}", DeleteBOM).RequireAuthorization("BOMDelete");
        
        // Get products that can be manufactured (have BOMs)
        group.MapGet("/manufactured-products", GetManufacturedProducts).RequireAuthorization("BOMView");
        
        // Get available components for a product
        group.MapGet("/available-components/{productId:int}", GetAvailableComponents).RequireAuthorization("BOMView");
        
        // Calculate material cost for a product
        group.MapGet("/material-cost/{productId:int}", GetMaterialCost).RequireAuthorization("BOMView");
    }

    private static async Task<IResult> GetAllBOMs(BatchIQDbContext db, int? parentProductId = null)
    {
        var query = db.ProductBOMs
            .Include(b => b.ParentProduct)
            .Include(b => b.ComponentProduct)
            .AsQueryable();

        if (parentProductId.HasValue)
        {
            query = query.Where(b => b.ParentProductId == parentProductId.Value);
        }

        var boms = await query
            .OrderBy(b => b.ParentProductId)
            .ThenBy(b => b.Sequence ?? int.MaxValue)
            .ToListAsync();

        var result = boms.Select(b => b.ToProductBOMResponse()).ToList();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetBOMByProduct(int productId, BatchIQDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Components)
                .ThenInclude(c => c.ComponentProduct)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return Results.NotFound($"Product with ID {productId} not found");

        var result = new
        {
            product = product.ToProductResponse(),
            components = product.Components
                .OrderBy(c => c.Sequence ?? int.MaxValue)
                .Select(c => c.ToProductBOMResponse())
                .ToList(),
            materialCost = product.Components.Where(c => c.CostPerUnit.HasValue)
                .Sum(c => c.QuantityRequired * c.CostPerUnit!.Value),
            canManufacture = product.IsManufactured && product.Components.Any()
        };

        return Results.Ok(result);
    }

    private static async Task<IResult> GetBOMById(int id, BatchIQDbContext db)
    {
        var bom = await db.ProductBOMs
            .Include(b => b.ParentProduct)
            .Include(b => b.ComponentProduct)
            .FirstOrDefaultAsync(b => b.Id == id);

        if (bom == null)
            return Results.NotFound($"BOM entry with ID {id} not found");

        return Results.Ok(bom.ToProductBOMDetailResponse());
    }

    private static async Task<IResult> CreateBOM(CreateProductBOMDto dto, BatchIQDbContext db)
    {
        // Validate parent product exists and is manufactured
        var parentProduct = await db.Products.FindAsync(dto.ParentProductId);
        if (parentProduct == null)
            return Results.BadRequest($"Parent product with ID {dto.ParentProductId} not found");

        // Validate component product exists
        var componentProduct = await db.Products.FindAsync(dto.ComponentProductId);
        if (componentProduct == null)
            return Results.BadRequest($"Component product with ID {dto.ComponentProductId} not found");

        // Prevent circular references
        if (dto.ParentProductId == dto.ComponentProductId)
            return Results.BadRequest("A product cannot be a component of itself");

        // Check if BOM entry already exists
        var existingBOM = await db.ProductBOMs
            .FirstOrDefaultAsync(b => b.ParentProductId == dto.ParentProductId && 
                                     b.ComponentProductId == dto.ComponentProductId);

        if (existingBOM != null)
            return Results.Conflict("BOM entry for this parent-component combination already exists");

        var bom = new ProductBOM
        {
            ParentProductId = dto.ParentProductId,
            ComponentProductId = dto.ComponentProductId,
            QuantityRequired = dto.QuantityRequired,
            Unit = dto.Unit,
            CostPerUnit = dto.CostPerUnit,
            IsCritical = dto.IsCritical,
            Notes = dto.Notes,
            Sequence = dto.Sequence
        };

        db.ProductBOMs.Add(bom);

        // Mark parent product as manufactured if not already
        if (!parentProduct.IsManufactured)
        {
            parentProduct.IsManufactured = true;
        }

        await db.SaveChangesAsync();

        // Reload with navigation properties
        await db.Entry(bom)
            .Reference(b => b.ParentProduct)
            .LoadAsync();
        await db.Entry(bom)
            .Reference(b => b.ComponentProduct)
            .LoadAsync();

        return Results.Created($"/api/product-bom/{bom.Id}", bom.ToProductBOMResponse());
    }

    private static async Task<IResult> UpdateBOM(int id, UpdateProductBOMDto dto, BatchIQDbContext db)
    {
        var bom = await db.ProductBOMs.FindAsync(id);
        if (bom == null)
            return Results.NotFound($"BOM entry with ID {id} not found");

        bom.QuantityRequired = dto.QuantityRequired;
        bom.Unit = dto.Unit;
        bom.CostPerUnit = dto.CostPerUnit;
        bom.IsCritical = dto.IsCritical;
        bom.Notes = dto.Notes;
        bom.Sequence = dto.Sequence;

        await db.SaveChangesAsync();

        // Reload with navigation properties
        await db.Entry(bom)
            .Reference(b => b.ParentProduct)
            .LoadAsync();
        await db.Entry(bom)
            .Reference(b => b.ComponentProduct)
            .LoadAsync();

        return Results.Ok(bom.ToProductBOMResponse());
    }

    private static async Task<IResult> DeleteBOM(int id, BatchIQDbContext db)
    {
        var bom = await db.ProductBOMs.FindAsync(id);
        if (bom == null)
            return Results.NotFound($"BOM entry with ID {id} not found");

        db.ProductBOMs.Remove(bom);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> GetManufacturedProducts(BatchIQDbContext db)
    {
        var products = await db.Products
            .Where(p => p.IsManufactured)
            .Include(p => p.Components)
                .ThenInclude(c => c.ComponentProduct)
            .ToListAsync();

        var result = products.Select(p => p.ToProductWithBOMResponse()).ToList();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetAvailableComponents(int productId, BatchIQDbContext db)
    {
        // Get all products except the specified product and any products that would create circular dependencies
        var existingComponentIds = await db.ProductBOMs
            .Where(b => b.ParentProductId == productId)
            .Select(b => b.ComponentProductId)
            .ToListAsync();

        var availableProducts = await db.Products
            .Where(p => p.Id != productId && !existingComponentIds.Contains(p.Id))
            .OrderBy(p => p.NameEn)
            .ToListAsync();

        var result = availableProducts.Select(p => new
        {
            id = p.Id,
            nameEn = p.NameEn,
            nameFa = p.NameFa,
            brandEn = p.BrandEn,
            brandFa = p.BrandFa,
            baseUnit = p.BaseUnit,
            currentPrice = p.Price,
            productType = p.ProductType
        }).ToList();

        return Results.Ok(result);
    }

    private static async Task<IResult> GetMaterialCost(int productId, BatchIQDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Components)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product == null)
            return Results.NotFound($"Product with ID {productId} not found");

        var materialCost = product.Components
            .Where(c => c.CostPerUnit.HasValue)
            .Sum(c => c.QuantityRequired * c.CostPerUnit!.Value);

        var componentCosts = product.Components.Select(c => new
        {
            componentId = c.ComponentProductId,
            componentName = c.ComponentProduct?.NameEn,
            quantityRequired = c.QuantityRequired,
            unit = c.Unit,
            costPerUnit = c.CostPerUnit,
            totalCost = c.CostPerUnit.HasValue ? c.QuantityRequired * c.CostPerUnit.Value : (decimal?)null
        }).ToList();

        var result = new
        {
            productId = product.Id,
            productName = product.NameEn,
            totalMaterialCost = materialCost,
            componentCosts = componentCosts,
            profitMargin = product.Price > materialCost ? product.Price - materialCost : 0,
            profitMarginPercentage = materialCost > 0 ? ((product.Price - materialCost) / materialCost * 100) : 0
        };

        return Results.Ok(result);
    }
}
