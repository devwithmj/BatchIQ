using BatchIQ.API.Models;
using BatchIQ.API.Services;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BatchIQ.API.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/users").WithTags("User Management");

        group.MapGet("/", GetAllUsers)
            .RequireAuthorization("UserView")
            .WithName("GetAllUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieve all users in the system")
            .Produces<List<UserDto>>(200);

        group.MapGet("/{id:int}", GetUserById)
            .RequireAuthorization("UserView")
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieve a specific user by their ID")
            .Produces<UserDto>(200)
            .Produces(404);

        group.MapPost("/", CreateUser)
            .RequireAuthorization("UserCreate")
            .WithName("CreateUser")
            .WithSummary("Create new user")
            .WithDescription("Create a new user account")
            .Produces<UserDto>(201)
            .Produces(400);

        group.MapPut("/{id:int}", UpdateUser)
            .RequireAuthorization("UserEdit")
            .WithName("UpdateUser")
            .WithSummary("Update user")
            .WithDescription("Update an existing user")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapDelete("/{id:int}", DeleteUser)
            .RequireAuthorization("UserDelete")
            .WithName("DeleteUser")
            .WithSummary("Delete user")
            .WithDescription("Delete a user account")
            .Produces(204)
            .Produces(404);

        group.MapPost("/{id:int}/assign-roles", AssignRoles)
            .RequireAuthorization("UserEdit")
            .WithName("AssignRoles")
            .WithSummary("Assign roles to user")
            .WithDescription("Assign or update roles for a specific user")
            .Produces(204)
            .Produces(400)
            .Produces(404);

        group.MapGet("/{id:int}/permissions", GetUserPermissions)
            .RequireAuthorization("UserView")
            .WithName("GetUserPermissions")
            .WithSummary("Get user permissions")
            .WithDescription("Get all permissions for a specific user")
            .Produces<List<PermissionDto>>(200)
            .Produces(404);
    }

    private static async Task<IResult> GetAllUsers(IUserService userService, bool includeInactive = false)
    {
        try
        {
            var users = await userService.GetAllUsersAsync(includeInactive);
            var result = users.Select(u => u.ToUserResponse()).ToList();
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to get users: {ex.Message}");
        }
    }

    private static async Task<IResult> GetUserById(int id, IUserService userService)
    {
        try
        {
            var user = await userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(user.ToUserDetailResponse());
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to get user: {ex.Message}");
        }
    }

    private static async Task<IResult> CreateUser(CreateUserDto dto, IUserService userService, ClaimsPrincipal currentUser)
    {
        try
        {
            var currentUserId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await userService.CreateUserAsync(dto, currentUserId);
            return Results.Created($"/api/users/{user.Id}", user.ToUserResponse());
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to create user: {ex.Message}");
        }
    }

    private static async Task<IResult> UpdateUser(int id, UpdateUserDto dto, IUserService userService, ClaimsPrincipal currentUser)
    {
        try
        {
            var currentUserId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var user = await userService.UpdateUserAsync(id, dto, currentUserId);
            return Results.Ok(user.ToUserResponse());
        }
        catch (InvalidOperationException ex)
        {
            return Results.NotFound();
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to update user: {ex.Message}");
        }
    }

    private static async Task<IResult> DeleteUser(int id, IUserService userService)
    {
        try
        {
            var success = await userService.DeleteUserAsync(id);
            if (!success)
            {
                return Results.NotFound();
            }

            return Results.NoContent();
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to delete user: {ex.Message}");
        }
    }

    private static async Task<IResult> AssignRoles(int id, AssignRoleDto dto, IUserService userService, ClaimsPrincipal currentUser)
    {
        try
        {
            var currentUserId = int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await userService.AssignRolesToUserAsync(dto.UserId, dto.RoleIds, currentUserId);
            
            if (!success)
            {
                return Results.NotFound();
            }

            return Results.Ok(new { message = "Roles assigned successfully" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to assign roles: {ex.Message}");
        }
    }

    private static async Task<IResult> GetUserPermissions(int id, IUserService userService)
    {
        try
        {
            var permissions = await userService.GetUserPermissionsAsync(id);
            return Results.Ok(new { userId = id, permissions });
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to get user permissions: {ex.Message}");
        }
    }
}
