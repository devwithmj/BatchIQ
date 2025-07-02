using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class PackagingJobConfiguration : IEntityTypeConfiguration<PackagingJob>
{
    public void Configure(EntityTypeBuilder<PackagingJob> builder)
    {
        builder.ToTable("PackagingJobs");

        builder.HasKey(job => job.Id);

        builder.Property(job => job.Id)
            .IsRequired();

        builder.Property(job => job.ProductId)
            .IsRequired();

        builder.Property(job => job.PackagingOptionId)
            .IsRequired();

        builder.Property(job => job.LocationId)
            .IsRequired();

        builder.Property(job => job.ExecutedOn)
            .IsRequired();

        builder.Property(job => job.InputQuantityKg)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Property(job => job.OutputUnits)
            .IsRequired();

        builder.Property(job => job.OperatorName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(job => job.CreatedOn)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(job => job.Product)
            .WithMany()
            .HasForeignKey(job => job.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(job => job.Location)
            .WithMany()
            .HasForeignKey(job => job.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
