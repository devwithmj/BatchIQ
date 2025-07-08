using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;

namespace BatchIQ.API.Endpoints;

public static class TransactionEndpoints
{
    public static void MapTransactionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/transactions").WithTags("Transactions");

        group.MapPost("/", CreateTransaction);
    }

    private static async Task<IResult> CreateTransaction(BatchIQDbContext db, TransactionDto dto)
    {
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
        
        return Results.Ok(transaction);
    }
}
