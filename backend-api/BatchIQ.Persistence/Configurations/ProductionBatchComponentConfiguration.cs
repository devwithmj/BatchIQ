using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductionBatchComponentConfiguration : IEntityTypeConfiguration<ProductionBatchComponent>
{
    public void Configure(EntityTypeBuilder<ProductionBatchComponent> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.ProductionBatchNumber)
            .IsRequired();

        builder.Property(x => x.ComponentProductId)
            .IsRequired();

        builder.Property(x => x.QuantityConsumed)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(x => x.UnitOfMeasure)
            .HasConversion<string>()
            .IsRequired();

        builder.HasOne(x => x.ComponentProduct)
            .WithMany()
            .HasForeignKey(x => x.ComponentProductId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
