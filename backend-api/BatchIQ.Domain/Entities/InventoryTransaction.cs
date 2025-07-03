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

    public decimal Quantity { get; set; }          // weight (kg) or units
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public TransactionType TransactionType { get; set; }

    // Optional batch / expiry tracking
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
}