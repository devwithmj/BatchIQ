using System;
using BatchIQ.Domain.Entities;
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

        e.Property(p => p.SizeValue).HasColumnType("decimal(10,2)");
        e.Property(p => p.Price).HasColumnType("decimal(12,2)");

        e.HasMany(p => p.Codes)
         .WithOne(c => c.Product)
         .HasForeignKey(c => c.ProductId)
         .OnDelete(DeleteBehavior.Cascade);
    }
}
