// PackagingOption entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class PackagingOption
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public decimal NetWeight { get; set; }    // e.g., 0.5 (kg) or 500 (g)
        public UoM UnitOfMeasure { get; set; }    // Weight vs Unit
        public string Label { get; set; } = default!; // “500 g bag”, “Single unit” …
    }
}
