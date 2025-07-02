// InventoryItem entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class InventoryItem
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!; // Navigation property to Product
    public Guid StockLocationId { get; set; }
    public StockLocation StockLocation { get; set; } = default!; // Navigation property to StockLocation
    
    public decimal Quantity { get; set; }
    public UoM UnitOfMeasure { get; set; }        // weight vs pcs

    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }      // link to ProductionBatch
}
}
