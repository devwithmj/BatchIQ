using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class BillOfMaterialLineConfiguration : IEntityTypeConfiguration<BillOfMaterialLine>
{
    public void Configure(EntityTypeBuilder<BillOfMaterialLine> builder)
    {
        builder.ToTable("BillOfMaterialLines");

        builder.HasKey(bomLine => bomLine.Id);

        builder.Property(bomLine => bomLine.BillOfMaterialId)
            .IsRequired();

        // builder.Property(bomLine => bomLine)
        //     .IsRequired();

        builder.Property(bomLine => bomLine.Quantity)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(bomLine => bomLine.BillOfMaterial)
            .WithMany(bom => bom.Lines)
            .HasForeignKey(bomLine => bomLine.BillOfMaterialId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(bomLine => bomLine.ComponentProduct)
            .WithMany()
            .HasForeignKey(bomLine => bomLine.ComponentProductId);
    }
}
