using System;
using System.Reflection;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.Persistence;

public class BatchIQDbContext : DbContext
{
    public BatchIQDbContext(DbContextOptions<BatchIQDbContext> options)
        : base(options)
    {

    }

    // ───── DbSets ───────────────────────────────────────────────
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCode> ProductCodes => Set<ProductCode>();
    public DbSet<PackagingOption> PackagingOptions => Set<PackagingOption>();

    public DbSet<BillOfMaterial> BillOfMaterials => Set<BillOfMaterial>();
    public DbSet<BillOfMaterialLine> BillOfMaterialLines => Set<BillOfMaterialLine>();

    public DbSet<StockLocation> StockLocations => Set<StockLocation>();
    public DbSet<InventoryItem> InventoryItems => Set<InventoryItem>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    public DbSet<ProductionBatch> ProductionBatches => Set<ProductionBatch>();
    public DbSet<ProductionBatchComponent> ProductionBatchComponents => Set<ProductionBatchComponent>();

    public DbSet<PackagingJob> PackagingJobs => Set<PackagingJob>();

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    // ────────────────────────────────────────────────────────────

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Apply all IEntityTypeConfiguration<T> found in this assembly
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        // Global decimal precision – works for SQLite & SQL Server
        foreach (var property in builder.Model
                 .GetEntityTypes()
                 .SelectMany(t => t.GetProperties())
                 .Where(p => p.ClrType == typeof(decimal)))
        {
            property.SetPrecision(18);
            property.SetScale(4);
        }
    }
}
