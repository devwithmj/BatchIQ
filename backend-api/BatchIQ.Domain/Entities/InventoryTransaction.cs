using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

public class InventoryTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }
    public Product? Product { get; set; }

    public int? FromLocationId { get; set; }  // null = “external in”
    public int? ToLocationId { get; set; }    // null = “external out”
    public Location? FromLocation { get; set; }
    public Location? ToLocation { get; set; }

    /// <summary>
    /// Quantity in the specified unit (not necessarily base unit)
    /// Example: 300 (when Unit = g), or 80 (when Unit = kg)
    /// </summary>
    public decimal Quantity { get; set; }

    /// <summary>
    /// Unit of measurement for this transaction
    /// Example: g (grams), kg (kilograms), Piece, etc.
    /// </summary>
    public SizeUnit Unit { get; set; }

    /// <summary>
    /// Quantity converted to the product's base unit for consistent calculations
    /// This is automatically calculated and stored for performance
    /// Example: If Quantity=80 and Unit=kg, BaseQuantity=80000 (grams)
    /// </summary>
    public decimal BaseQuantity { get; set; }

    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TransactionType TransactionType { get; set; }

    /// <summary>
    /// Reference to production transaction if this is related to manufacturing
    /// </summary>
    public int? ProductionTransactionId { get; set; }
    public ProductionTransaction? ProductionTransaction { get; set; }

    // Optional batch / expiry tracking
    public DateTime? ExpiryDate { get; set; }
    public string? BatchNumber { get; set; }
    public string? Notes { get; set; }

    /// <summary>
    /// Unit cost at the time of transaction (for cost tracking)
    /// </summary>
    public decimal? UnitCost { get; set; }
}