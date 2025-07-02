// InventoryTransaction entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class InventoryTransaction
{
    public Guid Id { get; set; }
    public DateTime OccurredOn { get; set; } = DateTime.UtcNow;

    public InventoryTxnType Type { get; set; }   // Move, Produce, Consume, Adjust

    public Guid? FromLocationId { get; set; }
    public Guid? ToLocationId   { get; set; }

    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public UoM UnitOfMeasure { get; set; }

    public string? Reference { get; set; }       // Order#, ProductionBatch#, etc.
}
}
