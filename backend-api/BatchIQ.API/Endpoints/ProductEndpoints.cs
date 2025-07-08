using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetAllProducts);
        group.MapPost("/", CreateProduct);
        group.MapPut("/{id:int}", UpdateProduct);
    }

    private static async Task<IResult> GetAllProducts(BatchIQDbContext db)
    {
        var products = await db.Products
            .Include(p => p.Codes)
            .ToListAsync();

        var result = products.Select(p => p.ToProductResponse()).ToList();
        return Results.Ok(result);
    }

    private static async Task<IResult> CreateProduct(BatchIQDbContext db, ProductDto dto)
    {
        var product = new Product
        {
            NameEn = dto.NameEn,
            NameFa = dto.NameFa,
            BrandEn = dto.BrandEn,
            BrandFa = dto.BrandFa,
            SizeValue = dto.SizeValue,
            UnitType = dto.UnitType,
            Price = dto.Price
        };

        db.Products.Add(product);
        await db.SaveChangesAsync();
        return Results.Created($"/api/products/{product.Id}", product);
    }

    private static async Task<IResult> UpdateProduct(int id, ProductDto dto, BatchIQDbContext db)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return Results.NotFound();

        product.NameEn = dto.NameEn;
        product.NameFa = dto.NameFa;
        product.BrandEn = dto.BrandEn;
        product.BrandFa = dto.BrandFa;
        product.SizeValue = dto.SizeValue;
        product.UnitType = dto.UnitType;
        product.Price = dto.Price;

        db.Products.Update(product);
        await db.SaveChangesAsync();

        db.ProductCodes.RemoveRange(db.ProductCodes.Where(c => c.ProductId == id));
        db.ProductCodes.AddRange(dto.Codes.Select(c => new ProductCode
        {
            Code = c,
            ProductId = id
        }));
        await db.SaveChangesAsync();
        return Results.NoContent();
    }
}
