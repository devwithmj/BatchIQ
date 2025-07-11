using BatchIQ.API.Models;
using BatchIQ.API.Services;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class RoleEndpoints
{
    public static void MapRoleEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/roles").WithTags("Role Management");

        group.MapGet("/", GetAllRoles)
            .RequireAuthorization("RoleView")
            .WithName("GetAllRoles")
            .WithSummary("Get all roles")
            .WithDescription("Retrieve all roles in the system with their permissions")
            .Produces<object[]>(200);

        group.MapGet("/{id:int}", GetRoleById)
            .RequireAuthorization("RoleView")
            .WithName("GetRoleById")
            .WithSummary("Get role by ID")
            .WithDescription("Retrieve a specific role by its ID with permissions")
            .Produces<object>(200)
            .Produces(404);
    }

    private static async Task<IResult> GetAllRoles(BatchIQDbContext context)
    {
        var roles = await context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.IsActive)
            .Select(r => new
            {
                id = r.Id,
                name = r.Name,
                description = r.Description,
                descriptionFa = r.DescriptionFa,
                isActive = r.IsActive,
                createdAt = r.CreatedAt,
                permissions = r.RolePermissions.Select(rp => new
                {
                    id = rp.Permission.Id,
                    name = rp.Permission.Name,
                    description = rp.Permission.Description,
                    category = rp.Permission.Category,
                    isActive = rp.Permission.IsActive
                }).ToList()
            })
            .ToListAsync();

        return Results.Ok(roles);
    }

    private static async Task<IResult> GetRoleById(int id, BatchIQDbContext context)
    {
        var role = await context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .Where(r => r.Id == id && r.IsActive)
            .Select(r => new
            {
                id = r.Id,
                name = r.Name,
                description = r.Description,
                descriptionFa = r.DescriptionFa,
                isActive = r.IsActive,
                createdAt = r.CreatedAt,
                permissions = r.RolePermissions.Select(rp => new
                {
                    id = rp.Permission.Id,
                    name = rp.Permission.Name,
                    description = rp.Permission.Description,
                    category = rp.Permission.Category,
                    isActive = rp.Permission.IsActive
                }).ToList()
            })
            .FirstOrDefaultAsync();

        return role is not null ? Results.Ok(role) : Results.NotFound();
    }
}
