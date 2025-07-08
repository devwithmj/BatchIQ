using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/transactions").WithTags("Transactions");

        group.MapGet("/", GetAllTransactions);
        group.MapGet("/{id:int}", GetTransactionById);
        group.MapPost("/", CreateTransaction);
        group.MapPut("/{id:int}", UpdateTransaction);
        group.MapDelete("/{id:int}", DeleteTransaction);
    }

    private static async Task<IResult> GetAllTransactions(BatchIQDbContext db)
    {
        var transactions = await db.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .OrderByDescending(t => t.Id)
            .ToListAsync();

        var result = transactions.Select(t => t.ToTransactionResponse()).ToList();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetTransactionById(int id, BatchIQDbContext db)
    {
        var transaction = await db.InventoryTransactions
            .Include(t => t.Product)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (transaction is null) return Results.NotFound();

        return Results.Ok(transaction.ToTransactionDetailResponse());
    }

    private static async Task<IResult> CreateTransaction(BatchIQDbContext db, TransactionDto dto)
    {
        // Validate product exists
        var productExists = await db.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
        {
            return Results.BadRequest("Product does not exist");
        }

        // Validate locations exist if provided
        if (dto.FromLocationId.HasValue)
        {
            var fromLocationExists = await db.Locations.AnyAsync(l => l.Id == dto.FromLocationId.Value);
            if (!fromLocationExists)
            {
                return Results.BadRequest("From location does not exist");
            }
        }

        if (dto.ToLocationId.HasValue)
        {
            var toLocationExists = await db.Locations.AnyAsync(l => l.Id == dto.ToLocationId.Value);
            if (!toLocationExists)
            {
                return Results.BadRequest("To location does not exist");
            }
        }

        // Validate quantity is positive
        if (dto.Quantity <= 0)
        {
            return Results.BadRequest("Quantity must be greater than zero");
        }

        var transaction = new InventoryTransaction
        {
            ProductId = dto.ProductId,
            FromLocationId = dto.FromLocationId,
            ToLocationId = dto.ToLocationId,
            Quantity = dto.Quantity,
            TransactionType = dto.TransactionType,
            ExpiryDate = dto.ExpiryDate
        };

        db.InventoryTransactions.Add(transaction);
        await db.SaveChangesAsync();
        
        return Results.Created($"/api/transactions/{transaction.Id}", transaction.ToTransactionCreatedResponse());
    }

    private static async Task<IResult> UpdateTransaction(int id, TransactionDto dto, BatchIQDbContext db)
    {
        var transaction = await db.InventoryTransactions.FindAsync(id);
        if (transaction is null) return Results.NotFound();

        // Validate product exists
        var productExists = await db.Products.AnyAsync(p => p.Id == dto.ProductId);
        if (!productExists)
        {
            return Results.BadRequest("Product does not exist");
        }

        // Validate locations exist if provided
        if (dto.FromLocationId.HasValue)
        {
            var fromLocationExists = await db.Locations.AnyAsync(l => l.Id == dto.FromLocationId.Value);
            if (!fromLocationExists)
            {
                return Results.BadRequest("From location does not exist");
            }
        }

        if (dto.ToLocationId.HasValue)
        {
            var toLocationExists = await db.Locations.AnyAsync(l => l.Id == dto.ToLocationId.Value);
            if (!toLocationExists)
            {
                return Results.BadRequest("To location does not exist");
            }
        }

        // Validate quantity is positive
        if (dto.Quantity <= 0)
        {
            return Results.BadRequest("Quantity must be greater than zero");
        }

        // Update transaction properties
        transaction.ProductId = dto.ProductId;
        transaction.FromLocationId = dto.FromLocationId;
        transaction.ToLocationId = dto.ToLocationId;
        transaction.Quantity = dto.Quantity;
        transaction.TransactionType = dto.TransactionType;
        transaction.ExpiryDate = dto.ExpiryDate;

        db.InventoryTransactions.Update(transaction);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteTransaction(int id, BatchIQDbContext db)
    {
        var transaction = await db.InventoryTransactions.FindAsync(id);
        
        if (transaction is null) return Results.NotFound();

        db.InventoryTransactions.Remove(transaction);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
