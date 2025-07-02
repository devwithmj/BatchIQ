using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductionBatchConfiguration : IEntityTypeConfiguration<ProductionBatch>
{
    public void Configure(EntityTypeBuilder<ProductionBatch> builder)
    {
        builder.HasKey(x => x.BatchNumber);

        builder.Property(x => x.BatchNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(x => x.ProducedOn)
            .IsRequired();

        builder.Property(x => x.YieldQuantity)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.UnitOfMeasure)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
