using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class PermissionEndpoints
{
    public static void MapPermissionEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/permissions").WithTags("Permission Management");

        group.MapGet("/", GetAllPermissions)
            .RequireAuthorization("RoleView") // Users with role management permissions can view permissions
            .WithName("GetAllPermissions")
            .WithSummary("Get all permissions")
            .WithDescription("Retrieve all permissions in the system grouped by category")
            .Produces<object[]>(200);

        group.MapGet("/categories", GetPermissionCategories)
            .RequireAuthorization("RoleView")
            .WithName("GetPermissionCategories")
            .WithSummary("Get permission categories")
            .WithDescription("Retrieve all permission categories with their permissions")
            .Produces<object[]>(200);

        group.MapGet("/{id:int}", GetPermissionById)
            .RequireAuthorization("RoleView")
            .WithName("GetPermissionById")
            .WithSummary("Get permission by ID")
            .WithDescription("Retrieve a specific permission by its ID")
            .Produces<object>(200)
            .Produces(404);

        group.MapGet("/roles/{roleId:int}", GetPermissionsByRole)
            .RequireAuthorization("RoleView")
            .WithName("GetPermissionsByRole")
            .WithSummary("Get permissions by role")
            .WithDescription("Retrieve all permissions assigned to a specific role")
            .Produces<object[]>(200)
            .Produces(404);

        group.MapPut("/{id:int}", UpdatePermission)
            .RequireAuthorization("RoleEdit") // Users with role edit permissions can update permissions
            .WithName("UpdatePermission")
            .WithSummary("Update permission")
            .WithDescription("Update an existing permission by its ID")
            .Produces<object>(200)
            .Produces(400)
            .Produces(404);

        group.MapPost("/", CreatePermission)
            .RequireAuthorization("RoleCreate") // Users with role create permissions can create permissions
            .WithName("CreatePermission")
            .WithSummary("Create new permission")
            .WithDescription("Create a new permission in the system")
            .Produces<object>(201)
            .Produces(400)
            .Produces(409);

        group.MapDelete("/{id:int}", DeletePermission)
            .RequireAuthorization("RoleDelete") // Users with role delete permissions can delete permissions
            .WithName("DeletePermission")
            .WithSummary("Delete permission")
            .WithDescription("Soft delete a permission by its ID (sets IsActive to false)")
            .Produces(204)
            .Produces(404)
            .Produces(409);
    }

    private static async Task<IResult> GetAllPermissions(BatchIQDbContext context)
    {
        var permissions = await context.Permissions
            .Where(p => p.IsActive)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                category = p.Category,
                isActive = p.IsActive
            })
            .OrderBy(p => p.category)
            .ThenBy(p => p.name)
            .ToListAsync();

        return Results.Ok(permissions);
    }

    private static async Task<IResult> GetPermissionCategories(BatchIQDbContext context)
    {
        var permissions = await context.Permissions
            .Where(p => p.IsActive)
            .ToListAsync();

        var categories = permissions
            .GroupBy(p => p.Category)
            .Select(g => new
            {
                category = g.Key,
                permissions = g.Select(p => new
                {
                    id = p.Id,
                    name = p.Name,
                    description = p.Description,
                    category = p.Category,
                    isActive = p.IsActive
                }).OrderBy(p => p.name).ToList()
            })
            .OrderBy(c => c.category)
            .ToList();

        return Results.Ok(categories);
    }

    private static async Task<IResult> GetPermissionById(int id, BatchIQDbContext context)
    {
        var permission = await context.Permissions
            .Where(p => p.Id == id && p.IsActive)
            .Select(p => new
            {
                id = p.Id,
                name = p.Name,
                description = p.Description,
                category = p.Category,
                isActive = p.IsActive
            })
            .FirstOrDefaultAsync();

        return permission is not null ? Results.Ok(permission) : Results.NotFound();
    }

    private static async Task<IResult> GetPermissionsByRole(int roleId, BatchIQDbContext context)
    {
        // Validate role exists
        var roleExists = await context.Roles.AnyAsync(r => r.Id == roleId && r.IsActive);
        if (!roleExists)
            return Results.NotFound("Role not found");

        var permissions = await context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Where(rp => rp.Permission.IsActive)
            .Select(rp => new
            {
                id = rp.Permission.Id,
                name = rp.Permission.Name,
                description = rp.Permission.Description,
                category = rp.Permission.Category,
                isActive = rp.Permission.IsActive
            })
            .OrderBy(p => p.category)
            .ThenBy(p => p.name)
            .ToListAsync();

        return Results.Ok(permissions);
    }

    private static async Task<IResult> UpdatePermission(int id, UpdatePermissionRequest request, BatchIQDbContext context)
    {
        // Validate permission exists
        var permission = await context.Permissions.FindAsync(id);
        if (permission is null || !permission.IsActive)
            return Results.NotFound();

        // Check if permission name already exists (exclude current permission)
        var nameExists = await context.Permissions
            .AnyAsync(p => p.Name == request.Name && p.Id != id && p.IsActive);
        if (nameExists)
            return Results.BadRequest("A permission with this name already exists");

        // Update permission properties
        permission.Name = request.Name;
        permission.Description = request.Description;
        permission.DescriptionFa = request.DescriptionFa;
        permission.Category = request.Category;
        permission.IsActive = request.IsActive;

        // Save changes
        await context.SaveChangesAsync();

        // Return updated permission
        var updatedPermission = new
        {
            id = permission.Id,
            name = permission.Name,
            description = permission.Description,
            descriptionFa = permission.DescriptionFa,
            category = permission.Category,
            isActive = permission.IsActive,
            createdAt = permission.CreatedAt
        };

        return Results.Ok(updatedPermission);
    }

    private static async Task<IResult> CreatePermission(CreatePermissionRequest request, BatchIQDbContext context)
    {
        // Validate required fields
        if (string.IsNullOrWhiteSpace(request.Name))
            return Results.BadRequest("Permission name is required");
            
        if (string.IsNullOrWhiteSpace(request.Category))
            return Results.BadRequest("Permission category is required");

        // Trim values to avoid whitespace issues
        var trimmedName = request.Name.Trim();
        var trimmedCategory = request.Category.Trim();

        // Check if a permission with the same name already exists
        var existingPermission = await context.Permissions
            .AnyAsync(p => p.Name == trimmedName && p.IsActive);
        if (existingPermission)
            return Results.Conflict("A permission with this name already exists");

        // Create a new permission
        var permission = new Permission
        {
            Name = trimmedName,
            Description = request.Description?.Trim(),
            DescriptionFa = request.DescriptionFa?.Trim(),
            Category = trimmedCategory,
            IsActive = true, // New permissions are active by default
            CreatedAt = DateTime.UtcNow
        };

        // Validate that the Category is not null before saving
        if (string.IsNullOrWhiteSpace(permission.Category))
        {
            return Results.BadRequest("Permission category cannot be null or empty");
        }

        context.Permissions.Add(permission);
        await context.SaveChangesAsync();

        // Return the created permission
        var createdPermission = new
        {
            id = permission.Id,
            name = permission.Name,
            description = permission.Description,
            descriptionFa = permission.DescriptionFa,
            category = permission.Category,
            isActive = permission.IsActive,
            createdAt = permission.CreatedAt
        };

        return Results.Created($"/api/permissions/{createdPermission.id}", createdPermission);
    }

    private static async Task<IResult> DeletePermission(int id, BatchIQDbContext context)
    {
        // Find the permission
        var permission = await context.Permissions.FindAsync(id);
        if (permission is null || !permission.IsActive)
            return Results.NotFound();

        // Check if permission is currently assigned to any roles
        var isAssignedToRoles = await context.RolePermissions
            .AnyAsync(rp => rp.PermissionId == id);
        
        if (isAssignedToRoles)
            return Results.Conflict("Cannot delete permission as it is currently assigned to one or more roles");

        // Soft delete - set IsActive to false instead of actual deletion
        permission.IsActive = false;
        
        await context.SaveChangesAsync();

        return Results.NoContent();
    }
}
