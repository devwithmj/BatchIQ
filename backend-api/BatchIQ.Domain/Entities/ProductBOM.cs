using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Entities;

/// <summary>
/// Bill of Materials - defines what components are needed to make a product
/// Example: Mixed Nuts requires 100g Pistachio, 150g Almond, 50g Cashew, etc.
/// </summary>
public class ProductBOM
{
    public int Id { get; set; }

    /// <summary>
    /// The finished product that is being made
    /// </summary>
    public int ParentProductId { get; set; }
    public Product ParentProduct { get; set; } = null!;

    /// <summary>
    /// The component/ingredient needed
    /// </summary>
    public int ComponentProductId { get; set; }
    public Product ComponentProduct { get; set; } = null!;

    /// <summary>
    /// How much of the component is needed (in base units)
    /// Example: 100 (grams of pistachio needed per unit of mixed nuts)
    /// </summary>
    public decimal QuantityRequired { get; set; }

    /// <summary>
    /// Unit of measurement for the required quantity
    /// Should typically match the component's base unit
    /// </summary>
    public SizeUnit Unit { get; set; }

    /// <summary>
    /// Optional: Cost per unit of this component at time of BOM creation
    /// </summary>
    public decimal? CostPerUnit { get; set; }

    /// <summary>
    /// Whether this component is critical (production cannot proceed without it)
    /// </summary>
    public bool IsCritical { get; set; } = true;

    /// <summary>
    /// Notes about this component in the recipe
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Sequence order for production (if relevant)
    /// </summary>
    public int? Sequence { get; set; }
}
