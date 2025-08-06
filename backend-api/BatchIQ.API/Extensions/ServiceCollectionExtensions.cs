using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using BatchIQ.API.Services;
using BatchIQ.API.Authorization;

namespace BatchIQ.API.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // 🔍 CONFIGURATION DEBUGGING
        Console.WriteLine("🔧 =================================");
        Console.WriteLine("🔧 CONFIGURATION DEBUG INFORMATION");
        Console.WriteLine("🔧 =================================");

        // Check which files are being loaded
        var configBuilder = configuration as IConfigurationBuilder;
        if (configuration is IConfigurationRoot configRoot)
        {
            Console.WriteLine("📁 Configuration Sources:");
            foreach (var source in configRoot.Providers)
            {
                Console.WriteLine($"   - {source.GetType().Name}: {source}");
            }
        }

        // Check current working directory
        Console.WriteLine($"📂 Current Directory: {Environment.CurrentDirectory}");
        Console.WriteLine($"🌍 Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Not Set"}");

        // Check specific JWT configuration values
        Console.WriteLine("🔑 JWT Configuration:");
        Console.WriteLine($"   Key: {(string.IsNullOrEmpty(configuration["Jwt:Key"]) ? "❌ NULL/EMPTY" : $"✅ Found ({configuration["Jwt:Key"]?.Length} chars)")}");
        Console.WriteLine($"   Issuer: {configuration["Jwt:Issuer"] ?? "❌ NULL"}");
        Console.WriteLine($"   Audience: {configuration["Jwt:Audience"] ?? "❌ NULL"}");
        Console.WriteLine($"   ExpiryHours: {configuration["Jwt:ExpiryHours"] ?? "❌ NULL"}");

        // Check database configuration
        Console.WriteLine("💾 Database Configuration:");
        Console.WriteLine($"   Provider: {configuration["DatabaseProvider"] ?? "❌ NULL"}");
        Console.WriteLine($"   Default Connection: {(string.IsNullOrEmpty(configuration.GetConnectionString("Default")) ? "❌ NULL" : "✅ Found")}");

        // Check if files exist
        var appSettingsPath = Path.Combine(Environment.CurrentDirectory, "appsettings.json");
        var appSettingsDev = Path.Combine(Environment.CurrentDirectory, "appsettings.Development.json");
        var appSettingsProd = Path.Combine(Environment.CurrentDirectory, "appsettings.Production.json");

        Console.WriteLine("📄 Configuration Files:");
        Console.WriteLine($"   appsettings.json: {(File.Exists(appSettingsPath) ? "✅ EXISTS" : "❌ MISSING")}");
        Console.WriteLine($"   appsettings.Development.json: {(File.Exists(appSettingsDev) ? "✅ EXISTS" : "❌ MISSING")}");
        Console.WriteLine($"   appsettings.Production.json: {(File.Exists(appSettingsProd) ? "✅ EXISTS" : "❌ MISSING")}");

        Console.WriteLine("🔧 =================================");

        // Database
        services.AddDbContext<BatchIQDbContext>(opt =>
            opt.UseSqlServer(configuration.GetConnectionString("Default") ?? "Server=(localdb)\\mssqllocaldb;Database=EFGetStarted.ConsoleApp.NewDb;Trusted_Connection=True;"));

        // API Documentation
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "BatchIQ API",
                Version = "v1",
                Description = "BatchIQ Manufacturing Management System API"
            });

            // Add JWT Authentication to Swagger
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                    },
                    new List<string>()
                }
            });
        });

        // CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                        "http://localhost:3000",
                        "https://localhost:3000",
                        "http://localhost:3001",
                        "https://localhost:3001",
                        "http://localhost:4200",
                        "https://localhost:4200",
                        "https://batchiq.site",
                        "https://www.batchiq.site",
                        "http://batchiq.site",
                        "http://www.batchiq.site"
                      )
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });

            // Add a more permissive policy for development
            options.AddPolicy("Development", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        // Authentication & Authorization Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        // JWT Authentication
        var jwtKey = configuration["Jwt:Key"] ?? "your-super-secret-key-that-is-at-least-32-characters-long";
        var key = Encoding.UTF8.GetBytes(jwtKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = configuration["Jwt:Issuer"] ?? "BatchIQ",
                ValidateAudience = true,
                ValidAudience = configuration["Jwt:Audience"] ?? "BatchIQ-Users",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };
        });

        // Authorization with custom policies
        services.AddCustomAuthorization();

        return services;
    }
}
