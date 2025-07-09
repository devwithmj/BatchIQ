using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

/// <summary>
/// Represents raw materials consumed in a production batch
/// Example: 80kg Raw Pistachio used in ROAST-2025-001
/// </summary>
public class BatchInput
{
    public int Id { get; set; }

    /// <summary>
    /// The production batch this input belongs to
    /// </summary>
    public int ProductionBatchId { get; set; }
    public ProductionBatch ProductionBatch { get; set; } = null!;

    /// <summary>
    /// The raw material product being consumed
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// How much of this material was used
    /// </summary>
    public decimal QuantityUsed { get; set; }

    /// <summary>
    /// Unit of measurement for the quantity
    /// </summary>
    public SizeUnit Unit { get; set; }

    /// <summary>
    /// Base quantity for consistent calculations
    /// </summary>
    public decimal BaseQuantityUsed { get; set; }

    /// <summary>
    /// Cost per unit of this material at time of production
    /// </summary>
    public decimal CostPerUnit { get; set; }

    /// <summary>
    /// Total cost for this input (QuantityUsed * CostPerUnit)
    /// </summary>
    public decimal TotalCost => QuantityUsed * CostPerUnit;

    /// <summary>
    /// Where this material was sourced from
    /// </summary>
    public int? SourceLocationId { get; set; }
    public Location? SourceLocation { get; set; }

    /// <summary>
    /// Batch number of the source material (for traceability)
    /// </summary>
    public string? SourceBatchNumber { get; set; }

    /// <summary>
    /// Notes about this specific input
    /// </summary>
    public string? Notes { get; set; }
}
