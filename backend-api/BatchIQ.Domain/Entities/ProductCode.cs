// ProductCode entity
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities
{
    public class ProductCode
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public Product Product { get; set; } = default!;

        public string Value { get; set; } = default!;
        public CodeType Type { get; set; }        // Barcode, PLU, SKU, etc.
        public bool IsPrimary { get; set; } = false;
    }
}
