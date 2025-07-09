using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

/// <summary>
/// Represents products created from a production batch
/// Example: 25kg Salted Pistachio produced from ROAST-2025-001
/// </summary>
public class BatchOutput
{
    public int Id { get; set; }

    /// <summary>
    /// The production batch this output belongs to
    /// </summary>
    public int ProductionBatchId { get; set; }
    public ProductionBatch ProductionBatch { get; set; } = null!;

    /// <summary>
    /// The finished product being created
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// How much of this product was produced
    /// </summary>
    public decimal QuantityProduced { get; set; }

    /// <summary>
    /// Unit of measurement for the quantity
    /// </summary>
    public SizeUnit Unit { get; set; }

    /// <summary>
    /// Base quantity for consistent calculations
    /// </summary>
    public decimal BaseQuantityProduced { get; set; }

    /// <summary>
    /// Yield percentage for this specific output
    /// (QuantityProduced / TotalInput * 100)
    /// </summary>
    public decimal? YieldPercentage { get; set; }

    /// <summary>
    /// Cost per unit allocated to this output
    /// </summary>
    public decimal? CostPerUnit { get; set; }

    /// <summary>
    /// Total allocated cost for this output
    /// </summary>
    public decimal? TotalAllocatedCost { get; set; }

    /// <summary>
    /// Where this output was placed
    /// </summary>
    public int? DestinationLocationId { get; set; }
    public Location? DestinationLocation { get; set; }

    /// <summary>
    /// Quality grade of this output
    /// </summary>
    public string? QualityGrade { get; set; }

    /// <summary>
    /// Expected vs actual quantity variance
    /// </summary>
    public decimal? QuantityVariance { get; set; }

    /// <summary>
    /// Notes about this specific output
    /// </summary>
    public string? Notes { get; set; }
}
