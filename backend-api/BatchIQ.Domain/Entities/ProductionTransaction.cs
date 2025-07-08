using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

/// <summary>
/// Represents a production/manufacturing transaction where raw materials are consumed
/// to produce finished or semi-finished products
/// </summary>
public class ProductionTransaction
{
    public int Id { get; set; }

    /// <summary>
    /// The product being manufactured
    /// </summary>
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    /// <summary>
    /// How much of the product was produced
    /// </summary>
    public decimal QuantityProduced { get; set; }

    /// <summary>
    /// Unit of the produced quantity
    /// </summary>
    public SizeUnit Unit { get; set; }

    /// <summary>
    /// Base unit quantity for consistent calculations
    /// </summary>
    public decimal BaseQuantityProduced { get; set; }

    /// <summary>
    /// Where the production took place
    /// </summary>
    public int LocationId { get; set; }
    public Location Location { get; set; } = null!;

    /// <summary>
    /// When the production was completed
    /// </summary>
    public DateTime ProductionDate { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Production batch number for traceability
    /// </summary>
    public string BatchNumber { get; set; } = null!;

    /// <summary>
    /// Status of the production
    /// </summary>
    public ProductionStatus Status { get; set; } = ProductionStatus.Planned;

    /// <summary>
    /// Total cost of materials used in this production
    /// </summary>
    public decimal? TotalMaterialCost { get; set; }

    /// <summary>
    /// Labor and overhead costs
    /// </summary>
    public decimal? ProductionCost { get; set; }

    /// <summary>
    /// All inventory transactions related to this production
    /// (both material consumption and finished product creation)
    /// </summary>
    public ICollection<InventoryTransaction> InventoryTransactions { get; set; } = new List<InventoryTransaction>();

    /// <summary>
    /// Notes about this production run
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Quality control information
    /// </summary>
    public string? QualityNotes { get; set; }

    /// <summary>
    /// Who initiated/supervised this production
    /// </summary>
    public string? SupervisorId { get; set; }
}
