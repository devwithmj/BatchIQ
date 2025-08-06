using BatchIQ.API.Endpoints;

namespace BatchIQ.API.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        // Use permissive CORS in development, restricted in production
        if (app.Environment.IsDevelopment())
        {
            app.UseCors("Development");
        }
        else
        {
            app.UseCors();
        }


        app.UseDeveloperExceptionPage();
        // Authentication & Authorization middleware
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }

    public static WebApplication MapEndpoints(this WebApplication app)
    {
        app.MapAuthEndpoints();
        app.MapUserEndpoints();
        app.MapRoleEndpoints();
        app.MapPermissionEndpoints();
        app.MapProductEndpoints();
        app.MapProductBOMEndpoints();
        app.MapLocationEndpoints();
        app.MapTransactionEndpoints();
        app.MapReportEndpoints();

        return app;
    }
}
