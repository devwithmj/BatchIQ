using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Services;

public interface IUserService
{
    Task<User?> GetUserByUsernameAsync(string username);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(int id);
    Task<User> CreateUserAsync(CreateUserDto dto, int? createdByUserId = null);
    Task<User> UpdateUserAsync(int id, UpdateUserDto dto, int? updatedByUserId = null);
    Task<bool> DeleteUserAsync(int id);
    Task<List<User>> GetAllUsersAsync(bool includeInactive = false);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds, int? assignedByUserId = null);
    Task<List<string>> GetUserPermissionsAsync(int userId);
    Task<bool> HasPermissionAsync(int userId, string permission);
}

public class UserService : IUserService
{
    private readonly BatchIQDbContext _context;
    private readonly IAuthService _authService;

    public UserService(BatchIQDbContext context, IAuthService authService)
    {
        _context = context;
        _authService = authService;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Email == email && u.IsActive);
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User> CreateUserAsync(CreateUserDto dto, int? createdByUserId = null)
    {
        // Check if username or email already exists
        if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException("Username already exists");

        if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email already exists");

        var passwordHash = await _authService.HashPasswordAsync(dto.Password);

        var user = new User
        {
            Username = dto.Username,
            Email = dto.Email,
            PasswordHash = passwordHash,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FirstNameFa = dto.FirstNameFa,
            LastNameFa = dto.LastNameFa,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Assign roles if provided
        if (dto.RoleIds != null && dto.RoleIds.Any())
        {
            await AssignRolesToUserAsync(user.Id, dto.RoleIds, createdByUserId);
        }

        return await GetUserByIdAsync(user.Id) ?? user;
    }

    public async Task<User> UpdateUserAsync(int id, UpdateUserDto dto, int? updatedByUserId = null)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            throw new InvalidOperationException("User not found");

        if (dto.FirstName != null) user.FirstName = dto.FirstName;
        if (dto.LastName != null) user.LastName = dto.LastName;
        if (dto.FirstNameFa != null) user.FirstNameFa = dto.FirstNameFa;
        if (dto.LastNameFa != null) user.LastNameFa = dto.LastNameFa;
        if (dto.IsActive.HasValue) user.IsActive = dto.IsActive.Value;
        
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Update roles if provided
        if (dto.RoleIds != null)
        {
            await AssignRolesToUserAsync(id, dto.RoleIds, updatedByUserId);
        }

        return await GetUserByIdAsync(id) ?? user;
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        user.IsActive = false;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<User>> GetAllUsersAsync(bool includeInactive = false)
    {
        var query = _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        return await query.ToListAsync();
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        if (!await _authService.VerifyPasswordAsync(currentPassword, user.PasswordHash))
            return false;

        user.PasswordHash = await _authService.HashPasswordAsync(newPassword);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignRolesToUserAsync(int userId, List<int> roleIds, int? assignedByUserId = null)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .FirstOrDefaultAsync(u => u.Id == userId);
        
        if (user == null) return false;

        // Remove existing roles
        _context.UserRoles.RemoveRange(user.UserRoles);

        // Add new roles
        foreach (var roleId in roleIds)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role != null && role.IsActive)
            {
                var userRole = new UserRole
                {
                    UserId = userId,
                    RoleId = roleId,
                    AssignedAt = DateTime.UtcNow,
                    AssignedByUserId = assignedByUserId
                };
                _context.UserRoles.Add(userRole);
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<string>> GetUserPermissionsAsync(int userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null) return new List<string>();

        return await _authService.GetUserPermissionsAsync(user);
    }

    public async Task<bool> HasPermissionAsync(int userId, string permission)
    {
        var permissions = await GetUserPermissionsAsync(userId);
        return permissions.Contains(permission);
    }
}
