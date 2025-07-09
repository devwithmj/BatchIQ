using System;
using System.ComponentModel.DataAnnotations;

namespace BatchIQ.Domain.Entities;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Username { get; set; } = null!;
    
    [Required]
    [StringLength(200)]
    public string Email { get; set; } = null!;
    
    [Required]
    public string PasswordHash { get; set; } = null!;
    
    [StringLength(100)]
    public string? FirstName { get; set; }
    
    [StringLength(100)]
    public string? LastName { get; set; }
    
    [StringLength(100)]
    public string? FirstNameFa { get; set; }
    
    [StringLength(100)]
    public string? LastNameFa { get; set; }
    
    public bool IsActive { get; set; } = true;
    public bool EmailConfirmed { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<Role> Roles { get; set; } = new List<Role>();
}
