using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> e)
    {
        e.ToTable("InventoryTransactions");
        e.HasKey(t => t.Id);

        e.Property(t => t.Quantity).HasColumnType("decimal(18,4)");
        e.HasIndex(t => t.Timestamp);   // quick filtering
    }
}