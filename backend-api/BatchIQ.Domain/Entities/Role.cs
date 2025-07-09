using System;
using System.ComponentModel.DataAnnotations;

namespace BatchIQ.Domain.Entities;

public class Role
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Name { get; set; } = null!;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    [StringLength(200)]
    public string? DescriptionFa { get; set; }
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
