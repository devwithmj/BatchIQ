using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class BatchInputConfiguration : IEntityTypeConfiguration<BatchInput>
{
    public void Configure(EntityTypeBuilder<BatchInput> e)
    {
        e.ToTable("BatchInputs");
        e.HasKey(bi => bi.Id);

        // Quantity fields
        e.Property(bi => bi.QuantityUsed).HasColumnType("decimal(18,4)");
        e.Property(bi => bi.BaseQuantityUsed).HasColumnType("decimal(18,4)");
        e.Property(bi => bi.CostPerUnit).HasColumnType("decimal(12,4)");

        // Unit field
        e.Property(bi => bi.Unit)
         .HasConversion<int>()
         .IsRequired();

        // Text fields
        e.Property(bi => bi.SourceBatchNumber).HasMaxLength(50);
        e.Property(bi => bi.Notes).HasMaxLength(500);

        // Relationships
        e.HasOne(bi => bi.ProductionBatch)
         .WithMany(pb => pb.Inputs)
         .HasForeignKey(bi => bi.ProductionBatchId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(bi => bi.Product)
         .WithMany(p => p.UsedAsInput)
         .HasForeignKey(bi => bi.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(bi => bi.SourceLocation)
         .WithMany()
         .HasForeignKey(bi => bi.SourceLocationId)
         .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        e.HasIndex(bi => bi.ProductionBatchId);
        e.HasIndex(bi => bi.ProductId);
        e.HasIndex(bi => bi.SourceBatchNumber);
    }
}

public class BatchOutputConfiguration : IEntityTypeConfiguration<BatchOutput>
{
    public void Configure(EntityTypeBuilder<BatchOutput> e)
    {
        e.ToTable("BatchOutputs");
        e.HasKey(bo => bo.Id);

        // Quantity fields
        e.Property(bo => bo.QuantityProduced).HasColumnType("decimal(18,4)");
        e.Property(bo => bo.BaseQuantityProduced).HasColumnType("decimal(18,4)");
        e.Property(bo => bo.YieldPercentage).HasColumnType("decimal(5,2)");
        e.Property(bo => bo.CostPerUnit).HasColumnType("decimal(12,4)");
        e.Property(bo => bo.TotalAllocatedCost).HasColumnType("decimal(12,2)");
        e.Property(bo => bo.QuantityVariance).HasColumnType("decimal(18,4)");

        // Unit field
        e.Property(bo => bo.Unit)
         .HasConversion<int>()
         .IsRequired();

        // Text fields
        e.Property(bo => bo.QualityGrade).HasMaxLength(20);
        e.Property(bo => bo.Notes).HasMaxLength(500);

        // Relationships
        e.HasOne(bo => bo.ProductionBatch)
         .WithMany(pb => pb.Outputs)
         .HasForeignKey(bo => bo.ProductionBatchId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(bo => bo.Product)
         .WithMany(p => p.ProducedAsOutput)
         .HasForeignKey(bo => bo.ProductId)
         .OnDelete(DeleteBehavior.Restrict);

        e.HasOne(bo => bo.DestinationLocation)
         .WithMany()
         .HasForeignKey(bo => bo.DestinationLocationId)
         .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        e.HasIndex(bo => bo.ProductionBatchId);
        e.HasIndex(bo => bo.ProductId);
        e.HasIndex(bo => bo.QualityGrade);
    }
}
