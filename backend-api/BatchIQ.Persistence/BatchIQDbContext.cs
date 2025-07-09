using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.Persistence;

public class BatchIQDbContext : DbContext
{
    public BatchIQDbContext(DbContextOptions<BatchIQDbContext> options) : base(options) { }

    // Core entities
    public DbSet<Product> Products => Set<Product>();
    public DbSet<ProductCode> ProductCodes => Set<ProductCode>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<InventoryTransaction> InventoryTransactions => Set<InventoryTransaction>();

    // Traditional BOM entities
    public DbSet<ProductBOM> ProductBOMs => Set<ProductBOM>();
    public DbSet<ProductionTransaction> ProductionTransactions => Set<ProductionTransaction>();

    // Process Manufacturing entities
    public DbSet<ProductionBatch> ProductionBatches => Set<ProductionBatch>();
    public DbSet<BatchInput> BatchInputs => Set<BatchInput>();
    public DbSet<BatchOutput> BatchOutputs => Set<BatchOutput>();

    // Process Templates
    public DbSet<ProcessTemplate> ProcessTemplates => Set<ProcessTemplate>();
    public DbSet<ProcessTemplateInput> ProcessTemplateInputs => Set<ProcessTemplateInput>();
    public DbSet<ProcessTemplateOutput> ProcessTemplateOutputs => Set<ProcessTemplateOutput>();

    // Authentication and Authorization entities
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BatchIQDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}