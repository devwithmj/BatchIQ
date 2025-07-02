// StockLocation entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class StockLocation
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;  // “Main Warehouse”, “Store-01”
        public bool IsWarehouse { get; set; }         // true = WH, false = retail store
        public bool IsCustomerFacing { get; set; }    // warehouse’s own customer area
        public LocationType Type { get; set; } = LocationType.Warehouse;
        public string? Description { get; set; }      // optional description
        public ICollection<Product> Products { get; set; } = new List<Product>(); // Navigation property to products in this location

    }
}
