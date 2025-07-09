using System;

namespace BatchIQ.Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }
    
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;
    
    public int PermissionId { get; set; }
    public Permission Permission { get; set; } = null!;
    
    public DateTime GrantedAt { get; set; } = DateTime.UtcNow;
    public int? GrantedByUserId { get; set; }
    public User? GrantedByUser { get; set; }
}
