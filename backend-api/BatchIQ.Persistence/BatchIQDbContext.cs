using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.Persistence;

public class BatchIQDbContext : DbContext
{
    public BatchIQDbContext(DbContextOptions<BatchIQDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCode> ProductCodes => Set<ProductCode>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(BatchIQDbContext).Assembly);
        base.OnModelCreating(b);
    }
}