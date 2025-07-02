// BillOfMaterial entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class BillOfMaterialLine
    {
        public Guid Id { get; set; }
        public Guid BillOfMaterialId { get; set; }
        public BillOfMaterial BillOfMaterial { get; set; } = default!;

        public Guid ComponentProductId { get; set; }
        public Product ComponentProduct { get; set; } = default!;

        public decimal Quantity { get; set; }            // 0.2 kg almond, etc.
        public UoM UnitOfMeasure { get; set; }
    }
}