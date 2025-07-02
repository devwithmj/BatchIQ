using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace BatchIQ.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<Order> builder)
    {
        
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerId)
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired()
            .HasColumnType("datetime");
        builder.Property(o => o.ShippedOn)
            .IsRequired(false) // Nullable if not yet shipped
            .HasColumnType("datetime");
        builder.Property(o => o.CompletedOn)
            .IsRequired(false) // Nullable if not yet completed
            .HasColumnType("datetime");
        builder.Property(o => o.CreatedOn)
            .IsRequired()
            .HasColumnType("datetime")
            .HasDefaultValueSql("GETUTCDATE()"); // Default to current UTC time

        builder.Property(o => o.Status)
            .IsRequired()
            .HasConversion<int>(); // Enum to int conversion

        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
