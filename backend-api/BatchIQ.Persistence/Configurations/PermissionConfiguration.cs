using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> e)
    {
        e.ToTable("Permissions");
        e.HasKey(p => p.Id);

        // Properties
        e.Property(p => p.Name)
         .IsRequired()
         .HasMaxLength(100);

        e.Property(p => p.Description)
         .HasMaxLength(200);

        e.Property(p => p.DescriptionFa)
         .HasMaxLength(200);

        e.Property(p => p.Category)
         .IsRequired()
         .HasMaxLength(50);

        e.Property(p => p.IsActive)
         .HasDefaultValue(true);

        e.Property(p => p.CreatedAt)
         .HasDefaultValueSql("datetime('now')")
         .IsRequired();

        // Relationships
        e.HasMany(p => p.RolePermissions)
         .WithOne(rp => rp.Permission)
         .HasForeignKey(rp => rp.PermissionId)
         .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        e.HasIndex(p => p.Name)
         .IsUnique();

        e.HasIndex(p => p.Category);
        e.HasIndex(p => p.IsActive);
    }
}
