using System;

namespace BatchIQ.Domain.Entities;

public class PackagingJob
{
    public Guid Id { get; set; }

    public Guid ProductId { get; set; }
    public Product Product { get; set; } = default!; // Navigation property to Product
    public Guid PackagingOptionId { get; set; } // Target retail option (e.g. 500 g)

    public Guid LocationId { get; set; }             // Warehouse OR Store doing the work
    public StockLocation Location { get; set; } = default!; // Navigation property to StockLocationß
    public DateTime ExecutedOn { get; set; } = DateTime.UtcNow;

    public decimal InputQuantityKg { get; set; }     // e.g. 25.0
    public int OutputUnits { get; set; }       // e.g. 50 bags

    public string OperatorName { get; set; } = default!;
    public DateTime? CreatedOn { get; set; } = null; // Nullable if not yet completed
}
