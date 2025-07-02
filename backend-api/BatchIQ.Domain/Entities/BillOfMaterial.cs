// BillOfMaterial entity
namespace BatchIQ.Domain.Entities
{
public class BillOfMaterial
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!;
    public string NameEn { get; set; } = default!; // English name
    public string? NameFa { get; set; } // Persian name, optional 

    // List of ingredients; each can reference another Product
        public ICollection<BillOfMaterialLine> Lines { get; set; } = new List<BillOfMaterialLine>();
}

}
