using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;          // “Main WH”, “Store A”, “Fridge C-3”
    public LocationType LocationType { get; set; }     // Warehouse | Store | Customer | Zone
    public int? ParentLocationId { get; set; }         // allows sub-locations
    public Location? ParentLocation { get; set; }

    public ICollection<Location> Children { get; set; } = new List<Location>();
}