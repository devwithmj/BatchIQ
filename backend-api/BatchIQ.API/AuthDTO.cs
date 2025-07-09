using BatchIQ.Domain.Enums;

namespace BatchIQ.API;

// Authentication DTOs
public record LoginDto(
    string Username,
    string Password
);

public record LoginResponseDto(
    string Token,
    string RefreshToken,
    UserDto User,
    List<string> Roles,
    List<string> Permissions
);

public record UserDto(
    int Id,
    string Username,
    string Email,
    string? FirstName,
    string? LastName,
    string? FirstNameFa,
    string? LastNameFa,
    bool IsActive,
    bool EmailConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    List<RoleDto> Roles
);

public record CreateUserDto(
    string Username,
    string Email,
    string Password,
    string? FirstName = null,
    string? LastName = null,
    string? FirstNameFa = null,
    string? LastNameFa = null,
    List<int>? RoleIds = null
);

public record UpdateUserDto(
    string? FirstName = null,
    string? LastName = null,
    string? FirstNameFa = null,
    string? LastNameFa = null,
    bool? IsActive = null,
    List<int>? RoleIds = null
);

public record ChangePasswordDto(
    string CurrentPassword,
    string NewPassword
);

public record RoleDto(
    int Id,
    string Name,
    string? Description,
    string? DescriptionFa,
    bool IsActive,
    List<PermissionDto> Permissions
);

public record CreateRoleDto(
    string Name,
    string? Description = null,
    string? DescriptionFa = null,
    List<int>? PermissionIds = null
);

public record UpdateRoleDto(
    string? Name = null,
    string? Description = null,
    string? DescriptionFa = null,
    bool? IsActive = null,
    List<int>? PermissionIds = null
);

public record PermissionDto(
    int Id,
    string Name,
    string? Description,
    string? DescriptionFa,
    string Category,
    bool IsActive
);

public record AssignRoleDto(
    int UserId,
    List<int> RoleIds
);

public record RefreshTokenDto(
    string RefreshToken
);
