using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

public class ProductCode
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string Code { get; set; } = null!;      // barcode, PLU, etc.
    public CodeType CodeType { get; set; }

    public Product? Product { get; set; }
}
