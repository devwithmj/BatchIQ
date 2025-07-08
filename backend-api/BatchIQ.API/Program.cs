using BatchIQ.API;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BatchIQDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=batchiq.db"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
                  .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/products", async (BatchIQDbContext db) =>
{
    var products = await db.Products
        .Include(p => p.Codes)
        .ToListAsync();

    var result = products.Select(p => new
    {
        id = p.Id,
        nameEn = p.NameEn,
        nameFa = p.NameFa,
        brandEn = p.BrandEn,
        brandFa = p.BrandFa,
        sizeValue = p.SizeValue,
        unitType = p.UnitType,
        price = p.Price,
        codes = p.Codes?.Select(c => c.Code).ToList() ?? new List<string>()
    }).ToList();

    return Results.Ok(result);
});

app.MapPost("/api/products", async (BatchIQDbContext db, ProductDto dto) =>
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
});
app.MapPut("/api/products/{id:int}", async (int id, ProductDto dto, BatchIQDbContext db) =>
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
});
app.MapPost("/api/transactions", async (BatchIQDbContext db, TransactionDto dto) =>
{
    var trx = new
    InventoryTransaction
    {
        ProductId = dto.ProductId,
        FromLocationId = dto.FromLocationId,
        ToLocationId = dto.ToLocationId,
        Quantity = dto.Quantity,
        TransactionType = dto.TransactionType,
        ExpiryDate = dto.ExpiryDate
    };
    db.InventoryTransactions.Add(trx);
    await db.SaveChangesAsync();
    return Results.Ok(trx);
});
app.UseCors();

app.Run();

