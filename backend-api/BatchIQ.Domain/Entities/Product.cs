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

    //-- manufacturing classification
    public ProductType ProductType { get; set; } = ProductType.Finished;
    public bool IsManufactured { get; set; } = false;  // True if this product is made from other products
    public bool IsProcessedProduct { get; set; } = false;  // True if this product comes from process manufacturing

    //-- size & unit (for packaging/display)
    public decimal SizeValue { get; set; }
    public SizeUnit UnitType { get; set; }   // Display unit (e.g., kg, piece)

    //-- inventory management units
    public SizeUnit BaseUnit { get; set; }   // Base unit for inventory calculations (e.g., g for weight-based products)

    //-- price
    public decimal Price { get; set; }       // sell price or SKU price

    //-- inventory management (optional enhancements)
    public decimal? MinimumStock { get; set; }     // Minimum inventory level
    public decimal? ReorderPoint { get; set; }     // When to reorder
    public bool IsActive { get; set; } = true;     // Product lifecycle management
    public bool IsAutoCreated { get; set; } = false;  // Flag for products created automatically via external price updates
    public bool IsExternallyUpdated { get; set; } = false;  // Flag for products updated via external API
    public DateTime? LastExternalUpdate { get; set; }  // When the product was last updated externally
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    //-- multiple codes
    public ICollection<ProductCode> Codes { get; set; } = new List<ProductCode>();
    
    //-- Traditional BOM (for assembly-type manufacturing like Mixed Nuts)
    public ICollection<ProductBOM> Components { get; set; } = new List<ProductBOM>();
    public ICollection<ProductBOM> UsedInProducts { get; set; } = new List<ProductBOM>();

    //-- Process Manufacturing (for transformation processes like Roasting)
    public ICollection<BatchInput> UsedAsInput { get; set; } = new List<BatchInput>();
    public ICollection<BatchOutput> ProducedAsOutput { get; set; } = new List<BatchOutput>();
}