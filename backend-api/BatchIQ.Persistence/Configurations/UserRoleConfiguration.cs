using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> e)
    {
        e.ToTable("UserRoles");
        e.HasKey(ur => ur.Id);

        // Properties
        e.Property(ur => ur.AssignedAt)
         .IsRequired();

        // Relationships
        e.HasOne(ur => ur.User)
         .WithMany(u => u.UserRoles)
         .HasForeignKey(ur => ur.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(ur => ur.Role)
         .WithMany(r => r.UserRoles)
         .HasForeignKey(ur => ur.RoleId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasOne(ur => ur.AssignedByUser)
         .WithMany()
         .HasForeignKey(ur => ur.AssignedByUserId)
         .OnDelete(DeleteBehavior.SetNull);

        // Indexes
        e.HasIndex(ur => new { ur.UserId, ur.RoleId })
         .IsUnique();

        e.HasIndex(ur => ur.AssignedAt);
    }
}
