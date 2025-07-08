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

    //-- product classification
    public ProductType ProductType { get; set; } = ProductType.Finished;

    //-- size & unit (for packaging/display)
    public decimal SizeValue { get; set; }
    public SizeUnit UnitType { get; set; }   // Display unit (e.g., kg, piece)

    //-- inventory management units
    public SizeUnit BaseUnit { get; set; }   // Base unit for inventory calculations (e.g., g for weight-based products)
    public bool IsManufactured { get; set; } = false;  // True if this product is made from other products

    //-- price
    public decimal Price { get; set; }       // sell price or SKU price

    //-- multiple codes
    public ICollection<ProductCode> Codes { get; set; } = new List<ProductCode>();
    
    //-- Bill of Materials (components needed to make this product)
    public ICollection<ProductBOM> Components { get; set; } = new List<ProductBOM>();
    
    //-- Products that use this as a component
    public ICollection<ProductBOM> UsedInProducts { get; set; } = new List<ProductBOM>();
}