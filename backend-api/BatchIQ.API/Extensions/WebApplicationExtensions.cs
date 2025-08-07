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
            app.UseCors(options=>
            {
                options.WithOrigins(
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
