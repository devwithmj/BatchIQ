using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductionTransactionConfiguration : IEntityTypeConfiguration<ProductionTransaction>
{
    public void Configure(EntityTypeBuilder<ProductionTransaction> e)
    {
        e.ToTable("ProductionTransactions");
        e.HasKey(pt => pt.Id);

        // Quantity fields
        e.Property(pt => pt.QuantityProduced).HasColumnType("decimal(18,4)");
        e.Property(pt => pt.BaseQuantityProduced).HasColumnType("decimal(18,4)");

        // Unit field
        e.Property(pt => pt.Unit)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.Piece);

        // Status field
        e.Property(pt => pt.Status)
         .HasConversion<int>()
         .HasDefaultValue(ProductionStatus.Planned);

        // Cost fields
        e.Property(pt => pt.TotalMaterialCost).HasColumnType("decimal(12,2)");
        e.Property(pt => pt.ProductionCost).HasColumnType("decimal(12,2)");

        // Text fields
        e.Property(pt => pt.BatchNumber)
         .IsRequired()
         .HasMaxLength(50);

        e.Property(pt => pt.Notes).HasMaxLength(1000);
        e.Property(pt => pt.QualityNotes).HasMaxLength(1000);
        e.Property(pt => pt.SupervisorId).HasMaxLength(50);

        // Relationships
        e.HasOne(pt => pt.Product)
         .WithMany()
         .HasForeignKey(pt => pt.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(pt => pt.Location)
         .WithMany()
         .HasForeignKey(pt => pt.LocationId)
         .OnDelete(DeleteBehavior.Restrict);

        // The InventoryTransactions relationship is already defined in InventoryTransactionConfiguration

        // Indexes for performance
        e.HasIndex(pt => pt.ProductionDate);
        e.HasIndex(pt => pt.ProductId);
        e.HasIndex(pt => pt.LocationId);
        e.HasIndex(pt => pt.Status);
        e.HasIndex(pt => pt.BatchNumber).IsUnique();
        e.HasIndex(pt => new { pt.ProductId, pt.ProductionDate });
        e.HasIndex(pt => new { pt.Status, pt.ProductionDate });
    }
}
