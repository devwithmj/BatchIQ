using BatchIQ.API;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<BatchIQDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("Default") ?? "Data Source=batchiq.db"));
var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/products", async (BatchIQDbContext db) =>
    await db.Products
            .Include(p => p.Codes)
            .ToListAsync());

app.MapPost("/api/products", async (BatchIQDbContext db, ProductDto dto) =>
{
    var product = new Product
    {
        PersianName = dto.PersianName,
        Brand       = dto.Brand,
        Size        = dto.Size,
        SizeUnit    = dto.SizeUnit
    };
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/api/products/{product.Id}", product);
});

app.MapPost("/api/transactions", async (BatchIQDbContext db, TransactionDto dto) =>
{
    var trx = new InventoryTransaction
    {
        ProductId     = dto.ProductId,
        FromLocationId= dto.FromLocationId,
        ToLocationId  = dto.ToLocationId,
        Quantity      = dto.Quantity,
        TransactionType = dto.TransactionType,
        ExpiryDate    = dto.ExpiryDate
    };
    db.InventoryTransactions.Add(trx);
    await db.SaveChangesAsync();
    return Results.Ok(trx);
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
