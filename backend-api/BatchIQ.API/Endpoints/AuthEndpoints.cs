using BatchIQ.API.Models;
using BatchIQ.API.Services;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BatchIQ.API.Endpoints;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Authenticate user")
            .WithDescription("Login with username and password to receive a JWT token")
            .Produces<LoginResponseDto>(200)
            .Produces(400)
            .Produces(401);

        group.MapPost("/register", Register)
            .RequireAuthorization("UserCreate")
            .WithName("Register")
            .WithSummary("Register new user")
            .WithDescription("Create a new user account (requires UserCreate permission)")
            .Produces<LoginResponseDto>(201)
            .Produces(400)
            .Produces(401)
            .Produces(403);

        group.MapPost("/refresh", RefreshToken)
            .WithName("RefreshToken")
            .WithSummary("Refresh JWT token")
            .WithDescription("Refresh an expired JWT token")
            .Produces<LoginResponseDto>(200)
            .Produces(400)
            .Produces(401);

        group.MapPost("/change-password", ChangePassword)
            .RequireAuthorization()
            .WithName("ChangePassword")
            .WithSummary("Change user password")
            .WithDescription("Change password for the authenticated user")
            .Produces(200)
            .Produces(400)
            .Produces(401);

        group.MapGet("/me", GetCurrentUser)
            .RequireAuthorization()
            .WithName("GetCurrentUser")
            .WithSummary("Get current user info")
            .WithDescription("Get information about the currently authenticated user")
            .Produces<UserDto>(200)
            .Produces(401);

        group.MapPost("/logout", Logout)
            .RequireAuthorization()
            .WithName("Logout")
            .WithSummary("Logout user")
            .WithDescription("Logout the current user and invalidate the token")
            .Produces(200)
            .Produces(401);
    }

    private static async Task<IResult> Login(LoginDto dto, IAuthService authService, IUserService userService, BatchIQDbContext db)
    {
        try
        {
            var user = await userService.GetUserByUsernameAsync(dto.Username);
            if (user == null)
            {
                return Results.Unauthorized();
            }

            if (!await authService.VerifyPasswordAsync(dto.Password, user.PasswordHash))
            {
                return Results.Unauthorized();
            }

            if (!user.IsActive)
            {
                return Results.BadRequest("Account is deactivated");
            }

            // Update last login
            user.LastLoginAt = DateTime.UtcNow;
            await db.SaveChangesAsync();

            // Generate tokens
            var token = await authService.GenerateJwtTokenAsync(user);
            var refreshToken = await authService.GenerateRefreshTokenAsync();
            var permissions = await authService.GetUserPermissionsAsync(user);

            var response = user.ToLoginResponse(token, refreshToken, permissions);
            return Results.Ok(response);
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Login failed: {ex.Message}");
        }
    }

    private static async Task<IResult> Register(CreateUserDto dto, IUserService userService, ClaimsPrincipal user)
    {
        try
        {
            var currentUserId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var newUser = await userService.CreateUserAsync(dto, currentUserId);
            return Results.Created($"/api/users/{newUser.Id}", newUser.ToUserResponse());
        }
        catch (InvalidOperationException ex)
        {
            return Results.BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Registration failed: {ex.Message}");
        }
    }

    private static async Task<IResult> RefreshToken(RefreshTokenDto dto)
    {
        // TODO: Implement refresh token logic with token storage
        return Results.BadRequest("Refresh token functionality not implemented yet");
    }

    private static async Task<IResult> ChangePassword(ChangePasswordDto dto, ClaimsPrincipal user, IUserService userService)
    {
        try
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var success = await userService.ChangePasswordAsync(userId, dto.CurrentPassword, dto.NewPassword);
            
            if (!success)
            {
                return Results.BadRequest("Current password is incorrect");
            }

            return Results.Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Password change failed: {ex.Message}");
        }
    }

    private static async Task<IResult> GetCurrentUser(ClaimsPrincipal user, IUserService userService)
    {
        try
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            var currentUser = await userService.GetUserByIdAsync(userId);
            
            if (currentUser == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(currentUser.ToUserDetailResponse());
        }
        catch (Exception ex)
        {
            return Results.BadRequest($"Failed to get user info: {ex.Message}");
        }
    }

    private static async Task<IResult> Logout()
    {
        // TODO: Implement token blacklisting if needed
        return Results.Ok(new { message = "Logged out successfully" });
    }
}
