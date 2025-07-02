using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductCodeConfiguration : IEntityTypeConfiguration<ProductCode>
{
    public void Configure(EntityTypeBuilder<ProductCode> b)
    {
        b.ToTable("ProductCodes");

        b.HasKey(pc => pc.Id);

        b.Property(pc => pc.Type)
         .HasConversion<int>();             // enum → int

        b.Property(pc => pc.Value)
         .IsRequired()
         .HasMaxLength(100);

        b.HasIndex(pc => new { pc.Value, pc.Type }).IsUnique();
    }
}
