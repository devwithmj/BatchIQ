using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetAllProducts).RequireAuthorization("ProductsView");
        group.MapGet("/{id:int}", GetProductById).RequireAuthorization("ProductsView");
        group.MapGet("/{id:int}/with-bom", GetProductWithBOM).RequireAuthorization("ProductsView");
        group.MapPost("/", CreateProduct).RequireAuthorization("ProductsCreate");
        group.MapPut("/{id:int}", UpdateProduct).RequireAuthorization("ProductsEdit");
        group.MapDelete("/{id:int}", DeleteProduct).RequireAuthorization("ProductsDelete");
    }

    private static async Task<IResult> GetAllProducts(BatchIQDbContext db, ProductType? productType = null, bool? isManufactured = null)
    {
        var query = db.Products
            .Include(p => p.Codes)
            .AsQueryable();

        if (productType.HasValue)
        {
            query = query.Where(p => p.ProductType == productType.Value);
        }

        if (isManufactured.HasValue)
        {
            query = query.Where(p => p.IsManufactured == isManufactured.Value);
        }

        var products = await query.ToListAsync();
        var result = products.Select(p => p.ToProductResponse()).ToList();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetProductById(int id, BatchIQDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Codes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return Results.NotFound($"Product with ID {id} not found");

        return Results.Ok(product.ToProductResponse());
    }

    private static async Task<IResult> GetProductWithBOM(int id, BatchIQDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Codes)
            .Include(p => p.Components)
                .ThenInclude(c => c.ComponentProduct)
            .Include(p => p.UsedInProducts)
                .ThenInclude(u => u.ParentProduct)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return Results.NotFound($"Product with ID {id} not found");

        return Results.Ok(product.ToProductWithBOMResponse());
    }

    private static async Task<IResult> CreateProduct(BatchIQDbContext db, CreateProductDto dto)
    {
        var product = new Product
        {
            NameEn = dto.NameEn,
            NameFa = dto.NameFa,
            BrandEn = dto.BrandEn,
            BrandFa = dto.BrandFa,
            ProductType = dto.ProductType ?? ProductType.Finished,
            SizeValue = dto.SizeValue,
            UnitType = dto.UnitType,
            BaseUnit = dto.BaseUnit ?? dto.UnitType,
            Price = dto.Price,
            IsManufactured = dto.IsManufactured ?? false
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();

        // Add product codes if provided
        if (dto.Codes?.Any() == true)
        {
            var productCodes = dto.Codes.Select(code => new ProductCode
            {
                Code = code,
                ProductId = product.Id,
                CodeType = CodeType.Barcode // Default, you might want to make this configurable
            }).ToList();

            db.ProductCodes.AddRange(productCodes);
            await db.SaveChangesAsync();
        }

        // Reload with codes
        await db.Entry(product)
            .Collection(p => p.Codes)
            .LoadAsync();

        return Results.Created($"/api/products/{product.Id}", product.ToProductResponse());
    }

    private static async Task<IResult> UpdateProduct(int id, UpdateProductDto dto, BatchIQDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Codes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return Results.NotFound($"Product with ID {id} not found");

        product.NameEn = dto.NameEn;
        product.NameFa = dto.NameFa;
        product.BrandEn = dto.BrandEn;
        product.BrandFa = dto.BrandFa;
        product.ProductType = dto.ProductType ?? product.ProductType;
        product.SizeValue = dto.SizeValue;
        product.UnitType = dto.UnitType;
        product.BaseUnit = dto.BaseUnit ?? product.BaseUnit;
        product.Price = dto.Price;
        product.IsManufactured = dto.IsManufactured ?? product.IsManufactured;

        // Update product codes
        if (dto.Codes != null)
        {
            // Remove existing codes
            db.ProductCodes.RemoveRange(product.Codes);

            // Add new codes
            if (dto.Codes.Any())
            {
                var productCodes = dto.Codes.Select(code => new ProductCode
                {
                    Code = code,
                    ProductId = id,
                    CodeType = CodeType.Barcode
                }).ToList();

                db.ProductCodes.AddRange(productCodes);
            }
        }

        await db.SaveChangesAsync();

        // Reload with codes
        await db.Entry(product)
            .Collection(p => p.Codes)
            .LoadAsync();

        return Results.Ok(product.ToProductResponse());
    }

    private static async Task<IResult> DeleteProduct(int id, BatchIQDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Components)
            .Include(p => p.UsedInProducts)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return Results.NotFound($"Product with ID {id} not found");

        // Check if product is used in any BOMs
        if (product.UsedInProducts.Any())
        {
            var parentProducts = string.Join(", ", product.UsedInProducts.Select(u => u.ParentProduct?.NameEn));
            return Results.BadRequest($"Cannot delete product because it is used as a component in: {parentProducts}");
        }

        // Check if there are any inventory transactions for this product
        var hasTransactions = await db.InventoryTransactions
            .AnyAsync(t => t.ProductId == id);

        if (hasTransactions)
        {
            return Results.BadRequest("Cannot delete product because it has inventory transaction history");
        }

        db.Products.Remove(product);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
