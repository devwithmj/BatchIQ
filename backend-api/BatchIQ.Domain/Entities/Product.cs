// Product entity
namespace BatchIQ.Domain.Entities
{
public class Product
{
    public Guid Id { get; set; }
    public string NameEn { get; set; } = default!;
    public string? NameFa { get; set; }

    // “Raw”, “Roasted”, “Mixed”, etc.
    public string Variant { get; set; } = default!;

    // Packaging defaults (e.g., 500 g bag, 1 kg box …)
    public ICollection<PackagingOption> PackagingOptions { get; set; } = new List<PackagingOption>();

    // One-to-many: a product may have many barcodes / PLUs / custom codes
    public ICollection<ProductCode> Codes { get; set; } = new List<ProductCode>();

    // Bill of Materials for mixes or roasted versions
    public BillOfMaterial? Bom { get; set; }

    // Soft-delete / Activity flags
    public bool IsActive { get; set; } = true;
}
}
