using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
{
    public void Configure(EntityTypeBuilder<RolePermission> e)
    {
        e.ToTable("RolePermissions");
        e.HasKey(rp => rp.Id);

        // Properties
        e.Property(rp => rp.GrantedAt)
         .HasDefaultValueSql("datetime('now')")
         .IsRequired();

        // Relationships
        e.HasOne(rp => rp.Role)
         .WithMany(r => r.RolePermissions)
         .HasForeignKey(rp => rp.RoleId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(rp => rp.Permission)
         .WithMany(p => p.RolePermissions)
         .HasForeignKey(rp => rp.PermissionId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(rp => rp.GrantedByUser)
         .WithMany()
         .HasForeignKey(rp => rp.GrantedByUserId)
         .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        e.HasIndex(rp => new { rp.RoleId, rp.PermissionId })
         .IsUnique();

        e.HasIndex(rp => rp.GrantedAt);
    }
}
