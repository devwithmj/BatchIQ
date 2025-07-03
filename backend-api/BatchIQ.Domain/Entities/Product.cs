using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

public class Product
{
    public int Id { get; set; }

    //-- names
    public string NameEn  { get; set; } = null!;
    public string NameFa  { get; set; } = null!;

    //-- brands
    public string BrandEn { get; set; } = null!;
    public string BrandFa { get; set; } = null!;

    //-- size & unit
    public decimal SizeValue { get; set; }
    public SizeUnit UnitType { get; set; }   // 0=g, 1=kg, …

    //-- price
    public decimal Price { get; set; }       // sell price or SKU price

    //-- multiple codes
    public ICollection<ProductCode> Codes { get; set; } = new List<ProductCode>();
}