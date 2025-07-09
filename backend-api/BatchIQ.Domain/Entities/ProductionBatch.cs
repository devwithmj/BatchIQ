using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

/// <summary>
/// Represents a production batch where raw materials are processed into multiple products
/// Example: 80kg Raw Pistachio → Salted, Sour, Saffron Roasted + Mixed Nuts ingredient
/// </summary>
public class ProductionBatch
{
    public int Id { get; set; }

    /// <summary>
    /// Unique batch identifier for traceability
    /// </summary>
    public string BatchNumber { get; set; } = null!;

    /// <summary>
    /// Type of production process
    /// </summary>
    public ProcessType ProcessType { get; set; }

    /// <summary>
    /// When the production started
    /// </summary>
    public DateTime ProductionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Where the production takes place
    /// </summary>
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    /// <summary>
    /// Current status of the production batch
    /// </summary>
    public ProductionStatus Status { get; set; } = ProductionStatus.Planned;

    /// <summary>
    /// Total cost of all input materials
    /// </summary>
    public decimal? TotalInputCost { get; set; }

    /// <summary>
    /// Labor and overhead costs for this batch
    /// </summary>
    public decimal? ProcessingCost { get; set; }

    /// <summary>
    /// Overall yield efficiency (total output / total input * 100)
    /// </summary>
    public decimal? YieldEfficiency { get; set; }

    /// <summary>
    /// Input materials consumed in this batch
    /// </summary>
    public ICollection<BatchInput> Inputs { get; set; } = new List<BatchInput>();

    /// <summary>
    /// Output products created from this batch
    /// </summary>
    public ICollection<BatchOutput> Outputs { get; set; } = new List<BatchOutput>();

    /// <summary>
    /// Quality control information
    /// </summary>
    public string? QualityNotes { get; set; }

    /// <summary>
    /// General notes about this production batch
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Who supervised this production
    /// </summary>
    public string? SupervisorId { get; set; }

    /// <summary>
    /// When the production was completed
    /// </summary>
    public DateTime? CompletionDate { get; set; }
}
