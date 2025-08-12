using BatchIQ.API.Models;
using BatchIQ.API;
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
        group.MapGet("/auto-created", GetAutoCreatedProducts).RequireAuthorization("ProductsView");
        group.MapGet("/externally-updated", GetExternallyUpdatedProducts).RequireAuthorization("ProductsView");
        group.MapGet("/{id:int}", GetProductById).RequireAuthorization("ProductsView");
        group.MapGet("/{id:int}/with-bom", GetProductWithBOM).RequireAuthorization("ProductsView");
        group.MapPost("/", CreateProduct).RequireAuthorization("ProductsCreate");
        group.MapPut("/{id:int}", UpdateProduct).RequireAuthorization("ProductsEdit");
        group.MapDelete("/{id:int}", DeleteProduct).RequireAuthorization("ProductsDelete");

        // External price update endpoint with API key authentication
        group.MapPut("/external/price-update", UpdateProductPriceByCode)
             .RequireAuthorization("PriceUpdateApiKey");

        // Clear external update tracking
        group.MapPut("/external/clear-tracking/{id:int}", ClearExternalUpdateTracking)
             .RequireAuthorization("ProductsEdit");
        group.MapPut("/external/clear-tracking-bulk", ClearExternalUpdateTrackingBulk)
             .RequireAuthorization("ProductsEdit");
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

    private static async Task<IResult> GetAutoCreatedProducts(BatchIQDbContext db)
    {
        var autoCreatedProducts = await db.Products
            .Include(p => p.Codes)
            .Where(p => p.IsAutoCreated && p.IsActive)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var result = autoCreatedProducts.Select(p => new
        {
            id = p.Id,
            nameEn = p.NameEn,
            nameFa = p.NameFa,
            brandEn = p.BrandEn,
            brandFa = p.BrandFa,
            price = p.Price,
            createdAt = p.CreatedAt,
            codes = p.Codes?.Select(c => c.Code).ToList() ?? new List<string>(),
            needsAttention = true
        }).ToList();

        return Results.Ok(new
        {
            count = result.Count,
            products = result
        });
    }

    private static async Task<IResult> GetExternallyUpdatedProducts(BatchIQDbContext db, DateTime? since = null, int? limit = null)
    {
        var query = db.Products
            .Include(p => p.Codes)
            .Where(p => p.IsExternallyUpdated && p.IsActive);

        // Filter by date if provided
        if (since.HasValue)
        {
            query = query.Where(p => p.LastExternalUpdate >= since.Value);
        }

        // Apply limit if provided
        if (limit.HasValue && limit.Value > 0)
        {
            query = query.Take(limit.Value);
        }

        var externallyUpdatedProducts = await query
            .OrderByDescending(p => p.LastExternalUpdate)
            .ToListAsync();

        var result = externallyUpdatedProducts.Select(p => new
        {
            id = p.Id,
            nameEn = p.NameEn,
            nameFa = p.NameFa,
            brandEn = p.BrandEn,
            brandFa = p.BrandFa,
            price = p.Price,
            isAutoCreated = p.IsAutoCreated,
            lastExternalUpdate = p.LastExternalUpdate,
            updatedAt = p.UpdatedAt,
            codes = p.Codes?.Select(c => c.Code).ToList() ?? new List<string>()
        }).ToList();

        return Results.Ok(new
        {
            count = result.Count,
            since = since,
            products = result
        });
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

    private static async Task<IResult> UpdateProductPriceByCode(ExternalPriceUpdateDto dto, BatchIQDbContext db)
    {
        try
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(dto.ProductCode))
            {
                return Results.BadRequest(new ExternalPriceUpdateResponseDto(
                    false,
                    "Product code is required",
                    dto.ProductCode ?? "",
                    null,
                    null,
                    false
                ));
            }

            if (dto.NewPrice < 0)
            {
                return Results.BadRequest(new ExternalPriceUpdateResponseDto(
                    false,
                    "Price cannot be negative",
                    dto.ProductCode,
                    null,
                    null,
                    false
                ));
            }

            // Find product by code
            var productCode = await db.ProductCodes
                .Include(pc => pc.Product)
                .FirstOrDefaultAsync(pc => pc.Code == dto.ProductCode);

            var isNewProduct = false;
            Product product;
            decimal? oldPrice = null;

            if (productCode?.Product != null)
            {
                // Existing product - update price
                product = productCode.Product;
                oldPrice = product.Price;
                product.Price = dto.NewPrice;
                product.UpdatedAt = DateTime.UtcNow;
                product.IsExternallyUpdated = true;
                product.LastExternalUpdate = DateTime.UtcNow;
            }
            else
            {
                // Product doesn't exist - create placeholder product
                isNewProduct = true;
                product = new Product
                {
                    NameEn = $"Auto-Created Product ({dto.ProductCode})",
                    NameFa = $"محصول ایجاد شده خودکار ({dto.ProductCode})",
                    BrandEn = "Unknown Brand",
                    BrandFa = "برند نامشخص",
                    ProductType = ProductType.Finished,
                    SizeValue = 1,
                    UnitType = SizeUnit.piece,
                    BaseUnit = SizeUnit.piece,
                    Price = dto.NewPrice,
                    IsActive = true,
                    IsAutoCreated = true,
                    IsExternallyUpdated = true,
                    LastExternalUpdate = DateTime.UtcNow,
                    CreatedAt = DateTime.UtcNow
                };

                db.Products.Add(product);
                await db.SaveChangesAsync(); // Save to get the product ID

                // Create the product code entry
                var newProductCode = new ProductCode
                {
                    ProductId = product.Id,
                    Code = dto.ProductCode,
                    CodeType = CodeType.Barcode // Default to barcode
                };

                db.ProductCodes.Add(newProductCode);
            }

            await db.SaveChangesAsync();

            var response = new ExternalPriceUpdateResponseDto(
                true,
                isNewProduct 
                    ? "Product created successfully with new price"
                    : "Product price updated successfully",
                dto.ProductCode,
                oldPrice,
                dto.NewPrice,
                isNewProduct
            );

            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            // Log the exception in a real application
            var errorResponse = new ExternalPriceUpdateResponseDto(
                false,
                $"An error occurred while updating the product price: {ex.Message}",
                dto.ProductCode ?? "",
                null,
                null,
                false
            );

            return Results.StatusCode(500);
        }
    }

    private static async Task<IResult> ClearExternalUpdateTracking(int id, BatchIQDbContext db)
    {
        var product = await db.Products.FindAsync(id);

        if (product == null)
            return Results.NotFound($"Product with ID {id} not found");

        product.IsExternallyUpdated = false;
        product.LastExternalUpdate = null;
        product.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            success = true,
            message = "External update tracking cleared for product",
            productId = id
        });
    }

    private static async Task<IResult> ClearExternalUpdateTrackingBulk(ClearExternalTrackingDto dto, BatchIQDbContext db)
    {
        var query = db.Products.Where(p => p.IsExternallyUpdated);

        // Filter by product IDs if provided
        if (dto.ProductIds?.Any() == true)
        {
            query = query.Where(p => dto.ProductIds.Contains(p.Id));
        }

        // Filter by date if provided
        if (dto.UpdatedSince.HasValue)
        {
            query = query.Where(p => p.LastExternalUpdate >= dto.UpdatedSince.Value);
        }

        var productsToUpdate = await query.ToListAsync();

        foreach (var product in productsToUpdate)
        {
            product.IsExternallyUpdated = false;
            product.LastExternalUpdate = null;
            product.UpdatedAt = DateTime.UtcNow;
        }

        await db.SaveChangesAsync();

        return Results.Ok(new
        {
            success = true,
            message = "External update tracking cleared for products",
            updatedCount = productsToUpdate.Count,
            productIds = productsToUpdate.Select(p => p.Id).ToList()
        });
    }
}
