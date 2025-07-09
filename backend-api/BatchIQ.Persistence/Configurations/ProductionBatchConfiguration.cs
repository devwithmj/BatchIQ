using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class ProductionBatchConfiguration : IEntityTypeConfiguration<ProductionBatch>
{
    public void Configure(EntityTypeBuilder<ProductionBatch> e)
    {
        e.ToTable("ProductionBatches");
        e.HasKey(pb => pb.Id);

        // Required fields
        e.Property(pb => pb.BatchNumber)
         .IsRequired()
         .HasMaxLength(50);

        e.Property(pb => pb.ProcessType)
         .HasConversion<int>()
         .IsRequired();

        e.Property(pb => pb.Status)
         .HasConversion<int>()
         .HasDefaultValue(ProductionStatus.Planned);

        // Cost fields
        e.Property(pb => pb.TotalInputCost).HasColumnType("decimal(12,2)");
        e.Property(pb => pb.ProcessingCost).HasColumnType("decimal(12,2)");
        e.Property(pb => pb.YieldEfficiency).HasColumnType("decimal(5,2)");

        // Text fields
        e.Property(pb => pb.QualityNotes).HasMaxLength(1000);
        e.Property(pb => pb.Notes).HasMaxLength(1000);
        e.Property(pb => pb.SupervisorId).HasMaxLength(50);

        // Relationships
        e.HasOne(pb => pb.Location)
         .WithMany()
         .HasForeignKey(pb => pb.LocationId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasMany(pb => pb.Inputs)
         .WithOne(bi => bi.ProductionBatch)
         .HasForeignKey(bi => bi.ProductionBatchId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(pb => pb.Outputs)
         .WithOne(bo => bo.ProductionBatch)
         .HasForeignKey(bo => bo.ProductionBatchId)
         .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        e.HasIndex(pb => pb.BatchNumber).IsUnique();
        e.HasIndex(pb => pb.ProductionDate);
        e.HasIndex(pb => pb.ProcessType);
        e.HasIndex(pb => pb.Status);
        e.HasIndex(pb => new { pb.ProcessType, pb.ProductionDate });
    }
}
