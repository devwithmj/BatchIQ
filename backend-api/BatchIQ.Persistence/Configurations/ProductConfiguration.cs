using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> b)
    {
        b.ToTable("Products");

        b.HasKey(p => p.Id);
        b.Property(p => p.NameEn).IsRequired().HasMaxLength(200);
        b.Property(p => p.NameFa).HasMaxLength(200);
        b.Property(p => p.Variant).HasMaxLength(100);

        b.HasMany(p => p.Codes)
         .WithOne(pc => pc.Product)
         .HasForeignKey(pc => pc.ProductId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasMany(p => p.PackagingOptions)
         .WithOne(po => po.Product)
         .HasForeignKey(po => po.ProductId)
         .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(p => p.Bom)
         .WithOne(bom => bom.Product)
         .HasForeignKey<BillOfMaterial>(bom => bom.ProductId);
    }
}
