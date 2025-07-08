using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductBOMConfiguration : IEntityTypeConfiguration<ProductBOM>
{
    public void Configure(EntityTypeBuilder<ProductBOM> e)
    {
        e.ToTable("ProductBOMs");
        e.HasKey(bom => bom.Id);

        // Quantity and cost fields
        e.Property(bom => bom.QuantityRequired).HasColumnType("decimal(18,4)");
        e.Property(bom => bom.CostPerUnit).HasColumnType("decimal(12,4)");

        // Unit field
        e.Property(bom => bom.Unit)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.Piece);

        // Flags and metadata
        e.Property(bom => bom.IsCritical)
         .HasDefaultValue(true);

        e.Property(bom => bom.Notes).HasMaxLength(500);
        e.Property(bom => bom.Sequence);

        // Relationships are already defined in ProductConfiguration
        // but we can add additional constraints here

        // Composite unique index to prevent duplicate BOM entries
        e.HasIndex(bom => new { bom.ParentProductId, bom.ComponentProductId })
         .IsUnique()
         .HasDatabaseName("IX_ProductBOM_Parent_Component");

        // Index for performance
        e.HasIndex(bom => bom.ParentProductId);
        e.HasIndex(bom => bom.ComponentProductId);
        e.HasIndex(bom => bom.IsCritical);
        e.HasIndex(bom => bom.Sequence);
    }
}
