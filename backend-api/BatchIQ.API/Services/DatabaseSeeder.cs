using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Services;

public interface IDatabaseSeeder
{
    Task SeedAsync();
}

public class DatabaseSeeder : IDatabaseSeeder
{
    private readonly BatchIQDbContext _context;
    private readonly IAuthService _authService;
    private readonly ILogger<DatabaseSeeder> _logger;

    public DatabaseSeeder(BatchIQDbContext context, IAuthService authService, ILogger<DatabaseSeeder> logger)
    {
        _context = context;
        _authService = authService;
        _logger = logger;
    }

    public async Task SeedAsync()
    {
        try
        {
            // Ensure database is created
            await _context.Database.EnsureCreatedAsync();

            // Check if any users exist
            if (await _context.Users.AnyAsync())
            {
                _logger.LogInformation("Database already contains users. Seeding skipped.");
                return;
            }

            _logger.LogInformation("Starting database seeding...");

            // Use a transaction to ensure all or nothing seeding
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Seed Permissions
                await SeedPermissionsAsync();
                
                // Seed Roles
                await SeedRolesAsync();
                
                // Save permissions and roles first
                await _context.SaveChangesAsync();
                
                // Seed Role Permissions (after roles and permissions are saved)
                await SeedRolePermissionsAsync();
                
                // Seed Admin User
                await SeedAdminUserAsync();
                
                // Save everything
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                _logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                _logger.LogError(ex, "An error occurred during database seeding. Transaction rolled back.");
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database.");
            throw;
        }
    }

    private async Task SeedPermissionsAsync()
    {
        var permissions = new List<Permission>();

        // Get all permission enum values
        var permissionEnums = Enum.GetValues<SystemPermission>();

        foreach (var permissionEnum in permissionEnums)
        {
            if (!await _context.Permissions.AnyAsync(p => p.Name == permissionEnum.ToString()))
            {
                var permission = new Permission
                {
                    Name = permissionEnum.ToString(),
                    Description = GetPermissionDescription(permissionEnum),
                    Category = GetPermissionCategory(permissionEnum),
                    IsActive = true
                };
                permissions.Add(permission);
            }
        }

        if (permissions.Any())
        {
            await _context.Permissions.AddRangeAsync(permissions);
            _logger.LogInformation($"Seeded {permissions.Count} permissions.");
        }
    }

    private async Task SeedRolesAsync()
    {
        var roles = new List<Role>();

        // SuperAdmin Role
        if (!await _context.Roles.AnyAsync(r => r.Name == SystemRole.SuperAdmin.ToString()))
        {
            var superAdminRole = new Role
            {
                Name = SystemRole.SuperAdmin.ToString(),
                Description = "Super Administrator with full system access",
                DescriptionFa = "مدیر ارشد با دسترسی کامل سیستم",
                IsActive = true
            };
            roles.Add(superAdminRole);
        }

        // Admin Role
        if (!await _context.Roles.AnyAsync(r => r.Name == SystemRole.Admin.ToString()))
        {
            var adminRole = new Role
            {
                Name = SystemRole.Admin.ToString(),
                Description = "Administrator with management access",
                DescriptionFa = "مدیر با دسترسی مدیریتی",
                IsActive = true
            };
            roles.Add(adminRole);
        }

        // Manager Role
        if (!await _context.Roles.AnyAsync(r => r.Name == SystemRole.Manager.ToString()))
        {
            var managerRole = new Role
            {
                Name = SystemRole.Manager.ToString(),
                Description = "Manager with operational access",
                DescriptionFa = "مدیر با دسترسی عملیاتی",
                IsActive = true
            };
            roles.Add(managerRole);
        }

        // Operator Role
        if (!await _context.Roles.AnyAsync(r => r.Name == SystemRole.Operator.ToString()))
        {
            var operatorRole = new Role
            {
                Name = SystemRole.Operator.ToString(),
                Description = "Operator with limited access",
                DescriptionFa = "اپراتور با دسترسی محدود",
                IsActive = true
            };
            roles.Add(operatorRole);
        }

        // Viewer Role
        if (!await _context.Roles.AnyAsync(r => r.Name == SystemRole.Viewer.ToString()))
        {
            var viewerRole = new Role
            {
                Name = SystemRole.Viewer.ToString(),
                Description = "Viewer with read-only access",
                DescriptionFa = "مشاهده‌گر با دسترسی فقط خواندنی",
                IsActive = true
            };
            roles.Add(viewerRole);
        }

        if (roles.Any())
        {
            await _context.Roles.AddRangeAsync(roles);
            _logger.LogInformation($"Seeded {roles.Count} roles.");
        }
    }

    private async Task SeedRolePermissionsAsync()
    {
        var rolePermissions = new List<RolePermission>();

        // Get role and permission mappings by name
        var superAdminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == SystemRole.SuperAdmin.ToString());
        var adminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == SystemRole.Admin.ToString());
        var managerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == SystemRole.Manager.ToString());
        var operatorRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == SystemRole.Operator.ToString());
        var viewerRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == SystemRole.Viewer.ToString());

        var allPermissions = await _context.Permissions.ToListAsync();

        // SuperAdmin gets all permissions
        if (superAdminRole != null)
        {
            foreach (var permission in allPermissions)
            {
                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRole.Id && rp.PermissionId == permission.Id))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = superAdminRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        // Admin gets most permissions (excluding system configuration)
        if (adminRole != null)
        {
            var adminPermissions = allPermissions.Where(p => 
                p.Name != SystemPermission.SystemConfiguration.ToString() &&
                p.Name != SystemPermission.SystemBackup.ToString() &&
                p.Name != SystemPermission.SystemRestore.ToString()).ToList();

            foreach (var permission in adminPermissions)
            {
                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == adminRole.Id && rp.PermissionId == permission.Id))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = adminRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        // Manager gets operational permissions
        if (managerRole != null)
        {
            var managerPermissionNames = new[]
            {
                SystemPermission.ProductsView.ToString(), SystemPermission.ProductsCreate.ToString(), SystemPermission.ProductsEdit.ToString(),
                SystemPermission.BOMView.ToString(), SystemPermission.BOMCreate.ToString(), SystemPermission.BOMEdit.ToString(),
                SystemPermission.InventoryView.ToString(), SystemPermission.InventoryCreate.ToString(), SystemPermission.InventoryEdit.ToString(),
                SystemPermission.ManufacturingView.ToString(), SystemPermission.ManufacturingCreate.ToString(), SystemPermission.ManufacturingEdit.ToString(),
                SystemPermission.ProductionBatchView.ToString(), SystemPermission.ProductionBatchCreate.ToString(), SystemPermission.ProductionBatchEdit.ToString(),
                SystemPermission.LocationView.ToString(), SystemPermission.LocationCreate.ToString(), SystemPermission.LocationEdit.ToString(),
                SystemPermission.ReportsView.ToString()
            };

            var managerPermissions = allPermissions.Where(p => managerPermissionNames.Contains(p.Name)).ToList();

            foreach (var permission in managerPermissions)
            {
                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == managerRole.Id && rp.PermissionId == permission.Id))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = managerRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        // Operator gets basic operational permissions
        if (operatorRole != null)
        {
            var operatorPermissionNames = new[]
            {
                SystemPermission.ProductsView.ToString(),
                SystemPermission.BOMView.ToString(),
                SystemPermission.InventoryView.ToString(), SystemPermission.InventoryCreate.ToString(),
                SystemPermission.ManufacturingView.ToString(),
                SystemPermission.ProductionBatchView.ToString(), SystemPermission.ProductionBatchCreate.ToString(),
                SystemPermission.LocationView.ToString()
            };

            var operatorPermissions = allPermissions.Where(p => operatorPermissionNames.Contains(p.Name)).ToList();

            foreach (var permission in operatorPermissions)
            {
                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == operatorRole.Id && rp.PermissionId == permission.Id))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = operatorRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        // Viewer gets only view permissions
        if (viewerRole != null)
        {
            var viewerPermissionNames = new[]
            {
                SystemPermission.ProductsView.ToString(),
                SystemPermission.BOMView.ToString(),
                SystemPermission.InventoryView.ToString(),
                SystemPermission.ManufacturingView.ToString(),
                SystemPermission.ProductionBatchView.ToString(),
                SystemPermission.LocationView.ToString(),
                SystemPermission.ReportsView.ToString()
            };

            var viewerPermissions = allPermissions.Where(p => viewerPermissionNames.Contains(p.Name)).ToList();

            foreach (var permission in viewerPermissions)
            {
                if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == viewerRole.Id && rp.PermissionId == permission.Id))
                {
                    rolePermissions.Add(new RolePermission
                    {
                        RoleId = viewerRole.Id,
                        PermissionId = permission.Id
                    });
                }
            }
        }

        if (rolePermissions.Any())
        {
            await _context.RolePermissions.AddRangeAsync(rolePermissions);
            _logger.LogInformation($"Seeded {rolePermissions.Count} role permissions.");
        }
    }

    private async Task SeedAdminUserAsync()
    {
        // Create default admin user
        var adminUser = new User
        {
            Username = "admin",
            Email = "admin@batchiq.com",
            FirstName = "System",
            LastName = "Administrator",
            FirstNameFa = "مدیر",
            LastNameFa = "سیستم",
            PasswordHash = await _authService.HashPasswordAsync("admin"),
            IsActive = true,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Users.AddAsync(adminUser);
        
        // Save to get the user ID
        await _context.SaveChangesAsync();

        // Get SuperAdmin role
        var superAdminRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == SystemRole.SuperAdmin.ToString());
        
        if (superAdminRole != null)
        {
            // Assign SuperAdmin role to admin user
            var userRole = new UserRole
            {
                UserId = adminUser.Id,
                RoleId = superAdminRole.Id,
                AssignedAt = DateTime.UtcNow
            };

            await _context.UserRoles.AddAsync(userRole);
        }
        
        _logger.LogInformation("Created default admin user with username 'admin' and password 'admin'.");
    }

    private static string GetPermissionDescription(SystemPermission permission)
    {
        return permission switch
        {
            SystemPermission.ProductsView => "View products",
            SystemPermission.ProductsCreate => "Create new products",
            SystemPermission.ProductsEdit => "Edit existing products",
            SystemPermission.ProductsDelete => "Delete products",
            SystemPermission.BOMView => "View bill of materials",
            SystemPermission.BOMCreate => "Create bill of materials",
            SystemPermission.BOMEdit => "Edit bill of materials",
            SystemPermission.BOMDelete => "Delete bill of materials",
            SystemPermission.InventoryView => "View inventory",
            SystemPermission.InventoryCreate => "Create inventory transactions",
            SystemPermission.InventoryEdit => "Edit inventory transactions",
            SystemPermission.InventoryDelete => "Delete inventory transactions",
            SystemPermission.ManufacturingView => "View manufacturing processes",
            SystemPermission.ManufacturingCreate => "Create manufacturing processes",
            SystemPermission.ManufacturingEdit => "Edit manufacturing processes",
            SystemPermission.ManufacturingDelete => "Delete manufacturing processes",
            SystemPermission.ProductionBatchView => "View production batches",
            SystemPermission.ProductionBatchCreate => "Create production batches",
            SystemPermission.ProductionBatchEdit => "Edit production batches",
            SystemPermission.ProductionBatchDelete => "Delete production batches",
            SystemPermission.LocationView => "View locations",
            SystemPermission.LocationCreate => "Create new locations",
            SystemPermission.LocationEdit => "Edit existing locations",
            SystemPermission.LocationDelete => "Delete locations",
            SystemPermission.UserView => "View users",
            SystemPermission.UserCreate => "Create new users",
            SystemPermission.UserEdit => "Edit existing users",
            SystemPermission.UserDelete => "Delete users",
            SystemPermission.RoleView => "View roles",
            SystemPermission.RoleCreate => "Create new roles",
            SystemPermission.RoleEdit => "Edit existing roles",
            SystemPermission.RoleDelete => "Delete roles",
            SystemPermission.ReportsView => "View reports",
            SystemPermission.ReportsExport => "Export reports",
            SystemPermission.SystemConfiguration => "Configure system settings",
            SystemPermission.SystemBackup => "Create system backups",
            SystemPermission.SystemRestore => "Restore system from backup",
            _ => permission.ToString()
        };
    }

    private static string GetPermissionCategory(SystemPermission permission)
    {
        return permission switch
        {
            SystemPermission.ProductsView or SystemPermission.ProductsCreate or 
            SystemPermission.ProductsEdit or SystemPermission.ProductsDelete => "Products",
            
            SystemPermission.BOMView or SystemPermission.BOMCreate or 
            SystemPermission.BOMEdit or SystemPermission.BOMDelete => "BOM",
            
            SystemPermission.InventoryView or SystemPermission.InventoryCreate or 
            SystemPermission.InventoryEdit or SystemPermission.InventoryDelete => "Inventory",
            
            SystemPermission.ManufacturingView or SystemPermission.ManufacturingCreate or 
            SystemPermission.ManufacturingEdit or SystemPermission.ManufacturingDelete => "Manufacturing",
            
            SystemPermission.ProductionBatchView or SystemPermission.ProductionBatchCreate or 
            SystemPermission.ProductionBatchEdit or SystemPermission.ProductionBatchDelete => "Production",
            
            SystemPermission.LocationView or SystemPermission.LocationCreate or 
            SystemPermission.LocationEdit or SystemPermission.LocationDelete => "Locations",
            
            SystemPermission.UserView or SystemPermission.UserCreate or 
            SystemPermission.UserEdit or SystemPermission.UserDelete => "Users",
            
            SystemPermission.RoleView or SystemPermission.RoleCreate or 
            SystemPermission.RoleEdit or SystemPermission.RoleDelete => "Roles",
            
            SystemPermission.ReportsView or SystemPermission.ReportsExport => "Reports",
            
            SystemPermission.SystemConfiguration or SystemPermission.SystemBackup or 
            SystemPermission.SystemRestore => "System",
            
            _ => "General"
        };
    }
}
