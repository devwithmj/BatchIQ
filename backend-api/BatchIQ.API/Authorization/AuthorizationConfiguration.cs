using Microsoft.AspNetCore.Authorization;

namespace BatchIQ.API.Authorization;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequirePermissionAttribute : Attribute, IAuthorizationRequirement
{
    public string Permission { get; }

    public RequirePermissionAttribute(string permission)
    {
        Permission = permission;
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class RequireApiKeyAttribute : Attribute, IAuthorizationRequirement
{
    public string ConfigKey { get; }

    public RequireApiKeyAttribute(string configKey = "ExternalApi:PriceUpdateApiKey")
    {
        ConfigKey = configKey;
    }
}

public class PermissionAuthorizationHandler : AuthorizationHandler<RequirePermissionAttribute>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequirePermissionAttribute requirement)
    {
        if (context.User.HasClaim("permission", requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class ApiKeyAuthorizationHandler : AuthorizationHandler<RequireApiKeyAttribute>
{
    private readonly IConfiguration _configuration;

    public ApiKeyAuthorizationHandler(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        RequireApiKeyAttribute requirement)
    {
        if (context.Resource is HttpContext httpContext)
        {
            var providedApiKey = httpContext.Request.Headers["X-API-Key"].FirstOrDefault();
            var expectedApiKey = _configuration[requirement.ConfigKey];

            if (!string.IsNullOrEmpty(providedApiKey) && 
                !string.IsNullOrEmpty(expectedApiKey) && 
                providedApiKey == expectedApiKey)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}

public static class AuthorizationExtensions
{
    public static void AddCustomAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            // Define policies for each permission
            options.AddPolicy("ProductsView", policy => 
                policy.RequireClaim("permission", "ProductsView"));
            options.AddPolicy("ProductsCreate", policy => 
                policy.RequireClaim("permission", "ProductsCreate"));
            options.AddPolicy("ProductsEdit", policy => 
                policy.RequireClaim("permission", "ProductsEdit"));
            options.AddPolicy("ProductsDelete", policy => 
                policy.RequireClaim("permission", "ProductsDelete"));

            options.AddPolicy("BOMView", policy => 
                policy.RequireClaim("permission", "BOMView"));
            options.AddPolicy("BOMCreate", policy => 
                policy.RequireClaim("permission", "BOMCreate"));
            options.AddPolicy("BOMEdit", policy => 
                policy.RequireClaim("permission", "BOMEdit"));
            options.AddPolicy("BOMDelete", policy => 
                policy.RequireClaim("permission", "BOMDelete"));

            options.AddPolicy("InventoryView", policy => 
                policy.RequireClaim("permission", "InventoryView"));
            options.AddPolicy("InventoryCreate", policy => 
                policy.RequireClaim("permission", "InventoryCreate"));
            options.AddPolicy("InventoryEdit", policy => 
                policy.RequireClaim("permission", "InventoryEdit"));
            options.AddPolicy("InventoryDelete", policy => 
                policy.RequireClaim("permission", "InventoryDelete"));

            options.AddPolicy("ManufacturingView", policy => 
                policy.RequireClaim("permission", "ManufacturingView"));
            options.AddPolicy("ManufacturingCreate", policy => 
                policy.RequireClaim("permission", "ManufacturingCreate"));
            options.AddPolicy("ManufacturingEdit", policy => 
                policy.RequireClaim("permission", "ManufacturingEdit"));
            options.AddPolicy("ManufacturingDelete", policy => 
                policy.RequireClaim("permission", "ManufacturingDelete"));

            options.AddPolicy("ProductionBatchView", policy => 
                policy.RequireClaim("permission", "ProductionBatchView"));
            options.AddPolicy("ProductionBatchCreate", policy => 
                policy.RequireClaim("permission", "ProductionBatchCreate"));
            options.AddPolicy("ProductionBatchEdit", policy => 
                policy.RequireClaim("permission", "ProductionBatchEdit"));
            options.AddPolicy("ProductionBatchDelete", policy => 
                policy.RequireClaim("permission", "ProductionBatchDelete"));

            options.AddPolicy("LocationView", policy => 
                policy.RequireClaim("permission", "LocationView"));
            options.AddPolicy("LocationCreate", policy => 
                policy.RequireClaim("permission", "LocationCreate"));
            options.AddPolicy("LocationEdit", policy => 
                policy.RequireClaim("permission", "LocationEdit"));
            options.AddPolicy("LocationDelete", policy => 
                policy.RequireClaim("permission", "LocationDelete"));

            options.AddPolicy("UserView", policy => 
                policy.RequireClaim("permission", "UserView"));
            options.AddPolicy("UserCreate", policy => 
                policy.RequireClaim("permission", "UserCreate"));
            options.AddPolicy("UserEdit", policy => 
                policy.RequireClaim("permission", "UserEdit"));
            options.AddPolicy("UserDelete", policy => 
                policy.RequireClaim("permission", "UserDelete"));

            options.AddPolicy("RoleView", policy => 
                policy.RequireClaim("permission", "RoleView"));
            options.AddPolicy("RoleCreate", policy => 
                policy.RequireClaim("permission", "RoleCreate"));
            options.AddPolicy("RoleEdit", policy => 
                policy.RequireClaim("permission", "RoleEdit"));
            options.AddPolicy("RoleDelete", policy => 
                policy.RequireClaim("permission", "RoleDelete"));

            options.AddPolicy("ReportsView", policy => 
                policy.RequireClaim("permission", "ReportsView"));
            options.AddPolicy("ReportsExport", policy => 
                policy.RequireClaim("permission", "ReportsExport"));

            options.AddPolicy("SystemConfiguration", policy => 
                policy.RequireClaim("permission", "SystemConfiguration"));

            // Role-based policies
            options.AddPolicy("SuperAdminOnly", policy => 
                policy.RequireRole("SuperAdmin"));
            options.AddPolicy("AdminOrAbove", policy => 
                policy.RequireRole("SuperAdmin", "Admin"));
            options.AddPolicy("ManagerOrAbove", policy => 
                policy.RequireRole("SuperAdmin", "Admin", "Manager"));

            // API Key based policies
            options.AddPolicy("PriceUpdateApiKey", policy =>
                policy.Requirements.Add(new RequireApiKeyAttribute()));
        });

        services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddScoped<IAuthorizationHandler, ApiKeyAuthorizationHandler>();
    }
}
