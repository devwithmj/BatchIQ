using System;
using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> e)
    {
        e.ToTable("Locations");
        e.HasKey(l => l.Id);

        e.Property(l => l.Name).IsRequired().HasMaxLength(128);

        e.HasOne(l => l.ParentLocation)
         .WithMany(l => l.Children)
         .HasForeignKey(l => l.ParentLocationId)
         .OnDelete(DeleteBehavior.Restrict);
    }
}