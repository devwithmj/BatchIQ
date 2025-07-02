using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BatchIQ.Persistence.Configurations;

public class PackagingOptionConfiguration : IEntityTypeConfiguration<PackagingOption>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<PackagingOption> builder)
    {
    builder.HasKey(po => po.Id);

    builder.Property(po => po.Id)
        .IsRequired();

    builder.Property(po => po.ProductId)
        .IsRequired();

    builder.HasOne(po => po.Product)
        .WithMany(p => p.PackagingOptions)
        .HasForeignKey(po => po.ProductId)
        .OnDelete(DeleteBehavior.Cascade);

    builder.Property(po => po.NetWeight)
        .IsRequired();

    builder.Property(po => po.UnitOfMeasure)
        .HasConversion<int>()
        .IsRequired();

    builder.Property(po => po.Label)
        .IsRequired()
        .HasMaxLength(100);
    }
}
