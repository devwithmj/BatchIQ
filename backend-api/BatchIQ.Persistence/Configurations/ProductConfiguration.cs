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

        // Size and unit properties
        e.Property(p => p.SizeValue).HasColumnType("decimal(10,2)");
        
        e.Property(p => p.UnitType)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.Piece);

        e.Property(p => p.BaseUnit)
         .HasConversion<int>()
         .HasDefaultValue(SizeUnit.Piece);

        e.Property(p => p.Price).HasColumnType("decimal(12,2)");

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

        // Indexes for performance
        e.HasIndex(p => p.ProductType);
        e.HasIndex(p => p.IsManufactured);
        e.HasIndex(p => new { p.NameEn, p.BrandEn });
        e.HasIndex(p => new { p.NameFa, p.BrandFa });
    }
}
