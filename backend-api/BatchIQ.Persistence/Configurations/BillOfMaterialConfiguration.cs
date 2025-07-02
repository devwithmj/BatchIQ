using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class BillOfMaterialConfiguration : IEntityTypeConfiguration<BillOfMaterial>
{
    public void Configure(EntityTypeBuilder<BillOfMaterial> builder)
    {
      
        builder.ToTable("BillOfMaterials");

        builder.HasKey(bom => bom.Id);

        builder.Property(bom => bom.ProductId)
            .IsRequired();

        builder.Property(bom => bom.NameEn)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(bom => bom.NameFa)
            .HasMaxLength(200);


        builder.HasOne(bom => bom.Product)
            .WithOne(p => p.Bom)
            .HasForeignKey<BillOfMaterial>(bom => bom.ProductId);
    }
}
