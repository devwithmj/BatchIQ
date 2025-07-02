using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class InventoryTransactionConfiguration : IEntityTypeConfiguration<InventoryTransaction>
{
    public void Configure(EntityTypeBuilder<InventoryTransaction> b)
    {
        b.ToTable("InventoryTransactions");

        b.HasKey(t => t.Id);

        b.Property(t => t.Type)
         .HasConversion<int>();

        b.Property(t => t.UnitOfMeasure)
         .HasConversion<int>();

        b.HasIndex(t => t.OccurredOn);

        // Self–FK pattern for move (nullable From/To)
        b.HasOne<StockLocation>()
         .WithMany()
         .HasForeignKey(t => t.FromLocationId)
         .OnDelete(DeleteBehavior.Restrict);

        b.HasOne<StockLocation>()
         .WithMany()
         .HasForeignKey(t => t.ToLocationId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}
