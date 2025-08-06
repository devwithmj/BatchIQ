using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BatchIQ.Domain.Entities;
using Microsoft.IdentityModel.Tokens;

namespace BatchIQ.API.Services;

public interface IAuthService
{
    Task<string> GenerateJwtTokenAsync(User user);
    Task<string> GenerateRefreshTokenAsync();
    Task<ClaimsPrincipal?> ValidateTokenAsync(string token);
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hash);
    Task<List<string>> GetUserPermissionsAsync(User user);
}

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly TokenValidationParameters _tokenValidationParameters;

    public AuthService(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        // Debug configuration loading
        var jwtKey = _configuration["Jwt:Key"] ??"ao/LI4hRduatLiKwX1rZK4BAWPZBPYKa0aVAFFs29BmRyx9ZJlWuXEoS8IwmegTjQPbX1/WIhhbswP/IvC5ndg==";
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        var jwtExpiryHours = _configuration["Jwt:ExpiryHours"];

        Console.WriteLine($"🔧 JWT Configuration Debug:");
        Console.WriteLine($"   Key: {(string.IsNullOrEmpty(jwtKey) ? "❌ NULL/EMPTY" : "✅ Loaded")}");
        Console.WriteLine($"   Issuer: {jwtIssuer ?? "❌ NULL"}");
        Console.WriteLine($"   Audience: {jwtAudience ?? "❌ NULL"}");
        Console.WriteLine($"   ExpiryHours: {jwtExpiryHours ?? "❌ NULL"}");

        if (string.IsNullOrEmpty(jwtKey))
        {
            throw new InvalidOperationException("JWT Key not configured in appsettings.json");
        }
        var key = Encoding.UTF8.GetBytes(jwtKey ?? throw new InvalidOperationException("JWT Key not configured"));


        _tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = _configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }

    public async Task<string> GenerateJwtTokenAsync(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new("firstName", user.FirstName ?? ""),
            new("lastName", user.LastName ?? ""),
            new("isActive", user.IsActive.ToString().ToLower())
        };

        // Add role claims
        foreach (var userRole in user.UserRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, userRole.Role.Name));
        }

        // Add permission claims
        var permissions = await GetUserPermissionsAsync(user);
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("permission", permission));
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(double.Parse(_configuration["Jwt:ExpiryHours"] ?? "1")),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<string> GenerateRefreshTokenAsync()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    public async Task<ClaimsPrincipal?> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, _tokenValidationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> HashPasswordAsync(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt());
    }

    public async Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }

    public async Task<List<string>> GetUserPermissionsAsync(User user)
    {
        var permissions = new HashSet<string>();

        foreach (var userRole in user.UserRoles)
        {
            foreach (var rolePermission in userRole.Role.RolePermissions)
            {
                if (rolePermission.Permission.IsActive)
                {
                    permissions.Add(rolePermission.Permission.Name);
                }
            }
        }

        return permissions.ToList();
    }
}
