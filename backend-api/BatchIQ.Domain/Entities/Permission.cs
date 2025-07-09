using System;
using System.ComponentModel.DataAnnotations;

namespace BatchIQ.Domain.Entities;

public class Permission
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = null!;
    
    [StringLength(200)]
    public string? Description { get; set; }
    
    [StringLength(200)]
    public string? DescriptionFa { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Category { get; set; } = null!; // e.g., "Products", "Inventory", "Manufacturing"
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
}
