using BatchIQ.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BatchIQ.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> e)
    {
        e.ToTable("Users");
        e.HasKey(u => u.Id);

        // Properties
        e.Property(u => u.Username)
         .IsRequired()
         .HasMaxLength(100);

        e.Property(u => u.Email)
         .IsRequired()
         .HasMaxLength(200);

        e.Property(u => u.PasswordHash)
         .IsRequired();

        e.Property(u => u.FirstName)
         .HasMaxLength(100);

        e.Property(u => u.LastName)
         .HasMaxLength(100);

        e.Property(u => u.FirstNameFa)
         .HasMaxLength(100);

        e.Property(u => u.LastNameFa)
         .HasMaxLength(100);

        e.Property(u => u.IsActive)
         .HasDefaultValue(true);

        e.Property(u => u.EmailConfirmed)
         .HasDefaultValue(false);

        e.Property(u => u.CreatedAt)
         .HasDefaultValueSql("datetime('now')")
         .IsRequired();

        e.Property(u => u.UpdatedAt)
         .IsRequired(false);

        e.Property(u => u.LastLoginAt)
         .IsRequired(false);

        // Relationships
        e.HasMany(u => u.UserRoles)
         .WithOne(ur => ur.User)
         .HasForeignKey(ur => ur.UserId)
         .OnDelete(DeleteBehavior.Cascade);

        e.HasMany(u => u.Roles)
         .WithMany(r => r.Users)
         .UsingEntity<UserRole>();

        // Indexes
        e.HasIndex(u => u.Username)
         .IsUnique();

        e.HasIndex(u => u.Email)
         .IsUnique();

        e.HasIndex(u => u.IsActive);
        e.HasIndex(u => u.CreatedAt);
    }
}
