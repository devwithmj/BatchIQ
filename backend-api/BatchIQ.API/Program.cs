using BatchIQ.API.Extensions;
using BatchIQ.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline
app.ConfigurePipeline();

// Map all endpoints
app.MapEndpoints();

// Seed the database
// using (var scope = app.Services.CreateScope())
// {
//     var seeder = scope.ServiceProvider.GetRequiredService<IDatabaseSeeder>();
//     await seeder.SeedAsync();
// }

app.Run();

