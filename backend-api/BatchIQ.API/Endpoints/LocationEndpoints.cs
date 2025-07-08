using BatchIQ.API.Models;
using BatchIQ.Domain.Entities;
using BatchIQ.Persistence;
using Microsoft.EntityFrameworkCore;

namespace BatchIQ.API.Endpoints;

public static class LocationEndpoints
{
    public static void MapLocationEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/locations").WithTags("Locations");

        group.MapGet("/", GetAllLocations);
        group.MapGet("/{id:int}", GetLocationById);
        group.MapPost("/", CreateLocation);
        group.MapPut("/{id:int}", UpdateLocation);
        group.MapDelete("/{id:int}", DeleteLocation);
    }

    private static async Task<IResult> GetAllLocations(BatchIQDbContext db)
    {
        var locations = await db.Locations
            .Include(l => l.ParentLocation)
            .Include(l => l.Children)
            .ToListAsync();

        var result = locations.Select(l => l.ToLocationResponse()).ToList();
        return Results.Ok(result);
    }

    private static async Task<IResult> GetLocationById(int id, BatchIQDbContext db)
    {
        var location = await db.Locations
            .Include(l => l.ParentLocation)
            .Include(l => l.Children)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location is null) return Results.NotFound();

        return Results.Ok(location.ToLocationDetailResponse());
    }

    private static async Task<IResult> CreateLocation(BatchIQDbContext db, LocationDto dto)
    {
        // Validate parent location exists if provided
        if (dto.ParentLocationId.HasValue)
        {
            var parentExists = await db.Locations.AnyAsync(l => l.Id == dto.ParentLocationId.Value);
            if (!parentExists)
            {
                return Results.BadRequest("Parent location does not exist");
            }
        }

        var location = new Location
        {
            Name = dto.Name,
            LocationType = dto.LocationType,
            ParentLocationId = dto.ParentLocationId
        };

        db.Locations.Add(location);
        await db.SaveChangesAsync();

        return Results.Created($"/api/locations/{location.Id}", location.ToLocationCreatedResponse());
    }

    private static async Task<IResult> UpdateLocation(int id, LocationDto dto, BatchIQDbContext db)
    {
        var location = await db.Locations.FindAsync(id);
        if (location is null) return Results.NotFound();

        // Validate parent location exists if provided and is different from current location
        if (dto.ParentLocationId.HasValue)
        {
            if (dto.ParentLocationId.Value == id)
            {
                return Results.BadRequest("Location cannot be its own parent");
            }

            var parentExists = await db.Locations.AnyAsync(l => l.Id == dto.ParentLocationId.Value);
            if (!parentExists)
            {
                return Results.BadRequest("Parent location does not exist");
            }

            // Check for circular reference
            if (await HasCircularReference(db, id, dto.ParentLocationId.Value))
            {
                return Results.BadRequest("Cannot create circular parent-child relationship");
            }
        }

        location.Name = dto.Name;
        location.LocationType = dto.LocationType;
        location.ParentLocationId = dto.ParentLocationId;

        db.Locations.Update(location);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> DeleteLocation(int id, BatchIQDbContext db)
    {
        var location = await db.Locations
            .Include(l => l.Children)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (location is null) return Results.NotFound();

        // Check if location has children
        if (location.Children.Any())
        {
            return Results.BadRequest("Cannot delete location with child locations. Please move or delete child locations first.");
        }

        // Check if location is used in any inventory transactions
        var hasTransactions = await db.InventoryTransactions
            .AnyAsync(t => t.FromLocationId == id || t.ToLocationId == id);

        if (hasTransactions)
        {
            return Results.BadRequest("Cannot delete location that has been used in inventory transactions.");
        }

        db.Locations.Remove(location);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<bool> HasCircularReference(BatchIQDbContext db, int locationId, int potentialParentId)
    {
        var potentialParent = await db.Locations.FindAsync(potentialParentId);
        var current = potentialParent;
        
        while (current?.ParentLocationId.HasValue == true)
        {
            if (current.ParentLocationId.Value == locationId)
            {
                return true;
            }
            current = await db.Locations.FindAsync(current.ParentLocationId.Value);
        }
        
        return false;
    }
}
