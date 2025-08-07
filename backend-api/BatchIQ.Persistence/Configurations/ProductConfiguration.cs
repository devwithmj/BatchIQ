using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> e)
    {
        e.ToTable("Products");
        e.HasKey(p => p.Id);

        e.Property(p => p.NameEn).IsRequired().HasMaxLength(128);
        e.Property(p => p.NameFa).IsRequired().HasMaxLength(128);
        e.Property(p => p.BrandEn).IsRequired().HasMaxLength(64);
        e.Property(p => p.BrandFa).IsRequired().HasMaxLength(64);

        // Product classification
        e.Property(p => p.ProductType)
         .HasConversion<int>()
         .HasDefaultValue(ProductType.Finished);

        e.Property(p => p.IsManufactured)
         .HasDefaultValue(false);

        e.Property(p => p.IsProcessedProduct)
         .HasDefaultValue(false);

        // Size and unit properties
        e.Property(p => p.SizeValue).HasColumnType("decimal(10,2)");
        
        e.Property(p => p.UnitType)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.piece);

        e.Property(p => p.BaseUnit)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.piece);

        e.Property(p => p.Price).HasColumnType("decimal(12,2)");

        // Enhanced inventory management properties
        e.Property(p => p.MinimumStock)
         .HasColumnType("decimal(12,2)")
         .IsRequired(false);

        e.Property(p => p.ReorderPoint)
         .HasColumnType("decimal(12,2)")
         .IsRequired(false);

        e.Property(p => p.IsActive)
         .HasDefaultValue(true);

        e.Property(p => p.CreatedAt)
         .IsRequired();

        e.Property(p => p.UpdatedAt)
         .IsRequired(false);

        // Relationships
        e.HasMany(p => p.Codes)
         .WithOne(c => c.Product)
         .HasForeignKey(c => c.ProductId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(p => p.Components)
         .WithOne(bom => bom.ParentProduct)
         .HasForeignKey(bom => bom.ParentProductId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(p => p.UsedInProducts)
         .WithOne(bom => bom.ComponentProduct)
         .HasForeignKey(bom => bom.ComponentProductId)
         .OnDelete(DeleteBehavior.Restrict); // Prevent deletion if product is used as component

        // Process Manufacturing relationships
        e.HasMany(p => p.UsedAsInput)
         .WithOne(bi => bi.Product)
         .HasForeignKey(bi => bi.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasMany(p => p.ProducedAsOutput)
         .WithOne(bo => bo.Product)
         .HasForeignKey(bo => bo.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        // Indexes for performance
        e.HasIndex(p => p.ProductType);
        e.HasIndex(p => p.IsManufactured);
        e.HasIndex(p => p.IsProcessedProduct);
        e.HasIndex(p => p.IsActive);
        e.HasIndex(p => p.CreatedAt);
        e.HasIndex(p => new { p.NameEn, p.BrandEn });
        e.HasIndex(p => new { p.NameFa, p.BrandFa });
    }
}
