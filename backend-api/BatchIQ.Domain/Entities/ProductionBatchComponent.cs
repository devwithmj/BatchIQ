// ProductionBatch entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class ProductionBatchComponent
    {
        public Guid Id { get; set; }
        public string ProductionBatchNumber { get; set; } = default!;
        public Guid ComponentProductId { get; set; }
        public Product ComponentProduct { get; set; } = default!; // Navigation property to Product
        public decimal QuantityConsumed { get; set; }
        public UoM UnitOfMeasure { get; set; }
    }
}
