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
            if (!await _context.Permissions.AnyAsync(p => p.Id == (int)permissionEnum))
            {
                var permission = new Permission
                {
                    Id = (int)permissionEnum,
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
        if (!await _context.Roles.AnyAsync(r => r.Id == (int)SystemRole.SuperAdmin))
        {
            var superAdminRole = new Role
            {
                Id = (int)SystemRole.SuperAdmin,
                Name = SystemRole.SuperAdmin.ToString(),
                Description = "Super Administrator with full system access",
                DescriptionFa = "مدیر ارشد با دسترسی کامل سیستم",
                IsActive = true
            };
            roles.Add(superAdminRole);
        }

        // Admin Role
        if (!await _context.Roles.AnyAsync(r => r.Id == (int)SystemRole.Admin))
        {
            var adminRole = new Role
            {
                Id = (int)SystemRole.Admin,
                Name = SystemRole.Admin.ToString(),
                Description = "Administrator with management access",
                DescriptionFa = "مدیر با دسترسی مدیریتی",
                IsActive = true
            };
            roles.Add(adminRole);
        }

        // Manager Role
        if (!await _context.Roles.AnyAsync(r => r.Id == (int)SystemRole.Manager))
        {
            var managerRole = new Role
            {
                Id = (int)SystemRole.Manager,
                Name = SystemRole.Manager.ToString(),
                Description = "Manager with operational access",
                DescriptionFa = "مدیر با دسترسی عملیاتی",
                IsActive = true
            };
            roles.Add(managerRole);
        }

        // Operator Role
        if (!await _context.Roles.AnyAsync(r => r.Id == (int)SystemRole.Operator))
        {
            var operatorRole = new Role
            {
                Id = (int)SystemRole.Operator,
                Name = SystemRole.Operator.ToString(),
                Description = "Operator with limited access",
                DescriptionFa = "اپراتور با دسترسی محدود",
                IsActive = true
            };
            roles.Add(operatorRole);
        }

        // Viewer Role
        if (!await _context.Roles.AnyAsync(r => r.Id == (int)SystemRole.Viewer))
        {
            var viewerRole = new Role
            {
                Id = (int)SystemRole.Viewer,
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

            // Now seed role permissions
            await SeedRolePermissionsAsync();
        }
    }

    private async Task SeedRolePermissionsAsync()
    {
        var rolePermissions = new List<RolePermission>();

        // SuperAdmin gets all permissions
        var allPermissions = await _context.Permissions.Select(p => p.Id).ToListAsync();
        var superAdminRoleId = (int)SystemRole.SuperAdmin;

        foreach (var permissionId in allPermissions)
        {
            if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == superAdminRoleId && rp.PermissionId == permissionId))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = superAdminRoleId,
                    PermissionId = permissionId
                });
            }
        }

        // Admin gets most permissions (excluding system configuration)
        var adminRoleId = (int)SystemRole.Admin;
        var adminPermissions = allPermissions.Where(p => 
            p != (int)SystemPermission.SystemConfiguration &&
            p != (int)SystemPermission.SystemBackup &&
            p != (int)SystemPermission.SystemRestore).ToList();

        foreach (var permissionId in adminPermissions)
        {
            if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == adminRoleId && rp.PermissionId == permissionId))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = adminRoleId,
                    PermissionId = permissionId
                });
            }
        }

        // Manager gets operational permissions
        var managerRoleId = (int)SystemRole.Manager;
        var managerPermissions = new[]
        {
            (int)SystemPermission.ProductsView, (int)SystemPermission.ProductsCreate, (int)SystemPermission.ProductsEdit,
            (int)SystemPermission.BOMView, (int)SystemPermission.BOMCreate, (int)SystemPermission.BOMEdit,
            (int)SystemPermission.InventoryView, (int)SystemPermission.InventoryCreate, (int)SystemPermission.InventoryEdit,
            (int)SystemPermission.ManufacturingView, (int)SystemPermission.ManufacturingCreate, (int)SystemPermission.ManufacturingEdit,
            (int)SystemPermission.ProductionBatchView, (int)SystemPermission.ProductionBatchCreate, (int)SystemPermission.ProductionBatchEdit,
            (int)SystemPermission.LocationView, (int)SystemPermission.LocationCreate, (int)SystemPermission.LocationEdit,
            (int)SystemPermission.ReportsView
        };

        foreach (var permissionId in managerPermissions)
        {
            if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == managerRoleId && rp.PermissionId == permissionId))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = managerRoleId,
                    PermissionId = permissionId
                });
            }
        }

        // Operator gets basic operational permissions
        var operatorRoleId = (int)SystemRole.Operator;
        var operatorPermissions = new[]
        {
            (int)SystemPermission.ProductsView,
            (int)SystemPermission.BOMView,
            (int)SystemPermission.InventoryView, (int)SystemPermission.InventoryCreate,
            (int)SystemPermission.ManufacturingView,
            (int)SystemPermission.ProductionBatchView, (int)SystemPermission.ProductionBatchCreate,
            (int)SystemPermission.LocationView
        };

        foreach (var permissionId in operatorPermissions)
        {
            if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == operatorRoleId && rp.PermissionId == permissionId))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = operatorRoleId,
                    PermissionId = permissionId
                });
            }
        }

        // Viewer gets only view permissions
        var viewerRoleId = (int)SystemRole.Viewer;
        var viewerPermissions = new[]
        {
            (int)SystemPermission.ProductsView,
            (int)SystemPermission.BOMView,
            (int)SystemPermission.InventoryView,
            (int)SystemPermission.ManufacturingView,
            (int)SystemPermission.ProductionBatchView,
            (int)SystemPermission.LocationView,
            (int)SystemPermission.ReportsView
        };

        foreach (var permissionId in viewerPermissions)
        {
            if (!await _context.RolePermissions.AnyAsync(rp => rp.RoleId == viewerRoleId && rp.PermissionId == permissionId))
            {
                rolePermissions.Add(new RolePermission
                {
                    RoleId = viewerRoleId,
                    PermissionId = permissionId
                });
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

        // Assign SuperAdmin role to admin user
        var userRole = new UserRole
        {
            UserId = adminUser.Id,
            RoleId = (int)SystemRole.SuperAdmin,
            AssignedAt = DateTime.UtcNow
        };

        await _context.UserRoles.AddAsync(userRole);
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
