using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> e)
    {
        e.ToTable("InventoryTransactions");
        e.HasKey(t => t.Id);

        // Quantity fields
        e.Property(t => t.Quantity).HasColumnType("decimal(18,4)");
        e.Property(t => t.BaseQuantity).HasColumnType("decimal(18,4)");
        
        // Timestamp field - use datetime2(3) for better SQL Server compatibility
        e.Property(t => t.Timestamp)
         .HasColumnType("datetime2(3)")
         .HasDefaultValueSql("GETUTCDATE()");
        
        // Unit field
        e.Property(t => t.Unit)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.Piece);

        // Cost tracking
        e.Property(t => t.UnitCost).HasColumnType("decimal(12,4)");

        // Batch tracking
        e.Property(t => t.BatchNumber).HasMaxLength(50);
        e.Property(t => t.Notes).HasMaxLength(500);

        // Relationships
        e.HasOne(t => t.Product)
         .WithMany()
         .HasForeignKey(t => t.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(t => t.FromLocation)
         .WithMany()
         .HasForeignKey(t => t.FromLocationId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(t => t.ToLocation)
         .WithMany()
         .HasForeignKey(t => t.ToLocationId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(t => t.ProductionTransaction)
         .WithMany(pt => pt.InventoryTransactions)
         .HasForeignKey(t => t.ProductionTransactionId)
         .OnDelete(DeleteBehavior.SetNull);

        // Indexes for performance
        e.HasIndex(t => t.Timestamp);
        e.HasIndex(t => t.ProductId);
        e.HasIndex(t => t.TransactionType);
        e.HasIndex(t => t.BatchNumber);
        e.HasIndex(t => new { t.ProductId, t.Timestamp });
        e.HasIndex(t => new { t.FromLocationId, t.ToLocationId });
    }
}