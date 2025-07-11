using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/stock-availability", GetStockAvailabilityReport)
            .RequireAuthorization("ReportsView")
            .WithName("GetStockAvailabilityReport")
            .WithSummary("Get stock availability report")
            .WithDescription("Retrieve stock availability report for all products with current stock levels, location breakdown, and reorder alerts")
            .Produces<List<StockAvailabilityReportDto>>(200);

        group.MapGet("/stock-availability/{productId:int}", GetProductStockAvailability)
            .RequireAuthorization("ReportsView")
            .WithName("GetProductStockAvailability")
            .WithSummary("Get stock availability for specific product")
            .WithDescription("Retrieve detailed stock availability for a specific product")
            .Produces<StockAvailabilityReportDto>(200)
            .Produces(404);

        group.MapGet("/product-transactions/{productId:int}", GetProductTransactionReport)
            .RequireAuthorization("ReportsView")
            .WithName("GetProductTransactionReport")
            .WithSummary("Get transaction report for specific product")
            .WithDescription("Retrieve all transactions for a specific product with optional date filtering")
            .Produces<List<ProductTransactionReportDto>>(200)
            .Produces(404);

        group.MapGet("/product-transactions/summary/{productId:int}", GetProductTransactionSummary)
            .RequireAuthorization("ReportsView")
            .WithName("GetProductTransactionSummary")
            .WithSummary("Get transaction summary for specific product")
            .WithDescription("Retrieve transaction summary with totals for a specific product")
            .Produces<TransactionSummaryDto>(200)
            .Produces(404);
    }

    private static async Task<IResult> GetStockAvailabilityReport(
        BatchIQDbContext context,
        bool? lowStockOnly = null,
        bool? needsReorderOnly = null)
    {
        var products = await context.Products
            .Where(p => p.IsActive)
            .ToListAsync();

        var stockReports = new List<StockAvailabilityReportDto>();

        foreach (var product in products)
        {
            // Calculate current stock from transactions
            var totalIn = await context.InventoryTransactions
                .Where(t => t.ProductId == product.Id && t.ToLocationId != null)
                .SumAsync(t => t.BaseQuantity);

            var totalOut = await context.InventoryTransactions
                .Where(t => t.ProductId == product.Id && t.FromLocationId != null)
                .SumAsync(t => t.BaseQuantity);

            var currentStock = totalIn - totalOut;

            // Calculate stock by location
            var locationStocks = await GetLocationStockBreakdown(context, product.Id);

            // Determine if low stock or needs reorder
            var isLowStock = product.MinimumStock.HasValue && currentStock <= product.MinimumStock.Value;
            var needsReorder = product.ReorderPoint.HasValue && currentStock <= product.ReorderPoint.Value;

            // Get last transaction date
            var lastTransaction = await context.InventoryTransactions
                .Where(t => t.ProductId == product.Id)
                .OrderByDescending(t => t.Timestamp)
                .Select(t => t.Timestamp)
                .FirstOrDefaultAsync();

            if (lastTransaction == default)
                lastTransaction = product.CreatedAt;

            var stockReport = new StockAvailabilityReportDto(
                product.Id,
                product.NameEn,
                product.NameFa,
                product.BrandEn,
                product.BrandFa,
                currentStock,
                product.BaseUnit.ToString(),
                product.MinimumStock,
                product.ReorderPoint,
                isLowStock,
                needsReorder,
                lastTransaction,
                locationStocks
            );

            // Apply filters
            if (lowStockOnly == true && !isLowStock) continue;
            if (needsReorderOnly == true && !needsReorder) continue;

            stockReports.Add(stockReport);
        }

        return Results.Ok(stockReports.OrderBy(r => r.ProductNameEn).ToList());
    }

    private static async Task<IResult> GetProductStockAvailability(int productId, BatchIQDbContext context)
    {
        var product = await context.Products
            .Where(p => p.Id == productId && p.IsActive)
            .FirstOrDefaultAsync();

        if (product == null)
            return Results.NotFound("Product not found");

        // Calculate current stock
        var totalIn = await context.InventoryTransactions
            .Where(t => t.ProductId == productId && t.ToLocationId != null)
            .SumAsync(t => t.BaseQuantity);

        var totalOut = await context.InventoryTransactions
            .Where(t => t.ProductId == productId && t.FromLocationId != null)
            .SumAsync(t => t.BaseQuantity);

        var currentStock = totalIn - totalOut;

        // Get location breakdown
        var locationStocks = await GetLocationStockBreakdown(context, productId);

        // Determine alerts
        var isLowStock = product.MinimumStock.HasValue && currentStock <= product.MinimumStock.Value;
        var needsReorder = product.ReorderPoint.HasValue && currentStock <= product.ReorderPoint.Value;

        // Get last transaction date
        var lastTransaction = await context.InventoryTransactions
            .Where(t => t.ProductId == productId)
            .OrderByDescending(t => t.Timestamp)
            .Select(t => t.Timestamp)
            .FirstOrDefaultAsync();

        if (lastTransaction == default)
            lastTransaction = product.CreatedAt;

        var stockReport = new StockAvailabilityReportDto(
            product.Id,
            product.NameEn,
            product.NameFa,
            product.BrandEn,
            product.BrandFa,
            currentStock,
            product.BaseUnit.ToString(),
            product.MinimumStock,
            product.ReorderPoint,
            isLowStock,
            needsReorder,
            lastTransaction,
            locationStocks
        );

        return Results.Ok(stockReport);
    }

    private static async Task<IResult> GetProductTransactionReport(
        int productId,
        BatchIQDbContext context,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        TransactionType? transactionType = null,
        int? locationId = null)
    {
        var product = await context.Products
            .Where(p => p.Id == productId && p.IsActive)
            .FirstOrDefaultAsync();

        if (product == null)
            return Results.NotFound("Product not found");

        var query = context.InventoryTransactions
            .Where(t => t.ProductId == productId)
            .Include(t => t.FromLocation)
            .Include(t => t.ToLocation)
            .AsQueryable();

        // Apply filters
        if (fromDate.HasValue)
            query = query.Where(t => t.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.Timestamp <= toDate.Value);

        if (transactionType.HasValue)
            query = query.Where(t => t.TransactionType == transactionType.Value);

        if (locationId.HasValue)
            query = query.Where(t => t.FromLocationId == locationId.Value || t.ToLocationId == locationId.Value);

        var transactions = await query
            .OrderByDescending(t => t.Timestamp)
            .ToListAsync();

        var transactionReports = transactions.Select(t => new ProductTransactionReportDto(
            t.Id,
            t.ProductId,
            product.NameEn,
            product.NameFa,
            t.FromLocation?.Name,
            t.ToLocation?.Name,
            t.Quantity,
            t.Unit.ToString(),
            t.BaseQuantity,
            product.BaseUnit.ToString(),
            t.TransactionType.ToString(),
            t.Timestamp,
            t.BatchNumber,
            t.ExpiryDate,
            t.Notes
        )).ToList();

        return Results.Ok(transactionReports);
    }

    private static async Task<IResult> GetProductTransactionSummary(
        int productId,
        BatchIQDbContext context,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var product = await context.Products
            .Where(p => p.Id == productId && p.IsActive)
            .FirstOrDefaultAsync();

        if (product == null)
            return Results.NotFound("Product not found");

        var query = context.InventoryTransactions
            .Where(t => t.ProductId == productId)
            .AsQueryable();

        // Apply date filters
        if (fromDate.HasValue)
            query = query.Where(t => t.Timestamp >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(t => t.Timestamp <= toDate.Value);

        var transactions = await query.ToListAsync();

        var totalIn = transactions
            .Where(t => t.ToLocationId != null)
            .Sum(t => t.BaseQuantity);

        var totalOut = transactions
            .Where(t => t.FromLocationId != null)
            .Sum(t => t.BaseQuantity);

        var netStock = totalIn - totalOut;

        var firstTransaction = transactions
            .OrderBy(t => t.Timestamp)
            .FirstOrDefault()?.Timestamp;

        var lastTransaction = transactions
            .OrderByDescending(t => t.Timestamp)
            .FirstOrDefault()?.Timestamp;

        var summary = new TransactionSummaryDto(
            productId,
            product.NameEn,
            product.NameFa,
            totalIn,
            totalOut,
            netStock,
            product.BaseUnit.ToString(),
            transactions.Count,
            firstTransaction,
            lastTransaction
        );

        return Results.Ok(summary);
    }

    private static async Task<List<LocationStockDto>> GetLocationStockBreakdown(BatchIQDbContext context, int productId)
    {
        var locations = await context.Locations.ToListAsync();
        var locationStocks = new List<LocationStockDto>();

        foreach (var location in locations)
        {
            var stockIn = await context.InventoryTransactions
                .Where(t => t.ProductId == productId && t.ToLocationId == location.Id)
                .SumAsync(t => t.BaseQuantity);

            var stockOut = await context.InventoryTransactions
                .Where(t => t.ProductId == productId && t.FromLocationId == location.Id)
                .SumAsync(t => t.BaseQuantity);

            var netStock = stockIn - stockOut;

            if (netStock > 0)
            {
                var product = await context.Products.FindAsync(productId);
                locationStocks.Add(new LocationStockDto(
                    location.Id,
                    location.Name,
                    netStock,
                    product?.BaseUnit.ToString() ?? "Units"
                ));
            }
        }

        return locationStocks.OrderBy(ls => ls.LocationName).ToList();
    }
}
