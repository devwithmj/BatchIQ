using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductCodeConfiguration : IEntityTypeConfiguration<ProductCode>
{
    public void Configure(EntityTypeBuilder<ProductCode> e)
    {
        e.ToTable("ProductCodes");
        e.HasKey(c => c.Id);

        e.Property(c => c.Code).IsRequired().HasMaxLength(64);
        e.HasIndex(c => c.Code).IsUnique();          // no duplicates
    }
}