using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    // Required core fields
    public string PersianName { get; set; } = null!;
    public string Brand { get; set; } = null!;
    public decimal Size { get; set; }          // e.g. 500, 1.0
    public SizeUnit SizeUnit { get; set; }     // g, kg, piece …

    // Relationships
    public ICollection<ProductCode> Codes { get; set; } = new List<ProductCode>();
}