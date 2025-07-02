// ProductionBatch entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class ProductionBatch
    {
        public string BatchNumber { get; set; } = default!;      // Human-friendly ID
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;        // The product being produced
        public DateTime ProducedOn { get; set; }

        public decimal YieldQuantity { get; set; }
        public UoM UnitOfMeasure { get; set; }

        // Navigation – which BOM lines were consumed
        public ICollection<ProductionBatchComponent> Components { get; set; } = new List<ProductionBatchComponent>();
    }
}
