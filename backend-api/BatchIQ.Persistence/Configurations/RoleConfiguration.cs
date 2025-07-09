using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> e)
    {
        e.ToTable("Roles");
        e.HasKey(r => r.Id);

        // Properties
        e.Property(r => r.Name)
         .IsRequired()
         .HasMaxLength(50);

        e.Property(r => r.Description)
         .HasMaxLength(200);

        e.Property(r => r.DescriptionFa)
         .HasMaxLength(200);

        e.Property(r => r.IsActive)
         .HasDefaultValue(true);

        e.Property(r => r.CreatedAt)
         .HasDefaultValueSql("datetime('now')")
         .IsRequired();

        // Relationships
        e.HasMany(r => r.UserRoles)
         .WithOne(ur => ur.Role)
         .HasForeignKey(ur => ur.RoleId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(r => r.RolePermissions)
         .WithOne(rp => rp.Role)
         .HasForeignKey(rp => rp.RoleId)
         .OnDelete(DeleteBehavior.Cascade);

        // Indexes
        e.HasIndex(r => r.Name)
         .IsUnique();

        e.HasIndex(r => r.IsActive);
    }
}
