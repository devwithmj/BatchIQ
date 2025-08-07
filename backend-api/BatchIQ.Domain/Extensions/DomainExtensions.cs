using System;
using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Enums;
using BatchIQ.Domain.Utilities;

namespace BatchIQ.Domain.Extensions;

/// <summary>
/// Extension methods for InventoryTransaction to handle unit conversions automatically
/// </summary>
public static class InventoryTransactionExtensions
{
    /// <summary>
    /// Sets the quantity and automatically calculates the base quantity
    /// </summary>
    public static void SetQuantity(this InventoryTransaction transaction, decimal quantity, SizeUnit unit, Product product)
    {
        transaction.Quantity = quantity;
        transaction.Unit = unit;
        transaction.BaseQuantity = UnitConverter.ConvertToBaseUnit(quantity, unit);
    }

    /// <summary>
    /// Gets the quantity in a specific unit
    /// </summary>
    public static decimal GetQuantityInUnit(this InventoryTransaction transaction, SizeUnit targetUnit)
    {
        return UnitConverter.Convert(transaction.Quantity, transaction.Unit, targetUnit);
    }

    /// <summary>
    /// Gets a human-readable string representation of the quantity
    /// </summary>
    public static string GetQuantityDisplay(this InventoryTransaction transaction)
    {
        return $"{transaction.Quantity:N2} {GetUnitDisplayName(transaction.Unit)}";
    }

    private static string GetUnitDisplayName(SizeUnit unit)
    {
        return unit switch
        {
            SizeUnit.gr => "grams",
            SizeUnit.kg => "kilograms",
            SizeUnit.ml => "milliliters",
            SizeUnit.l => "liters",
            SizeUnit.piece => "pieces",
            SizeUnit.pack => "packs",
            SizeUnit.box => "boxes",
            SizeUnit.other => "other",
            SizeUnit.lb => "pounds",
            SizeUnit.pkg => "packages",
            SizeUnit.plb => "pounds (lbs)",
            SizeUnit.phandered => "hundreds",
            SizeUnit.ea => "each",
            _ => "unknown",
        };
    }
}

/// <summary>
/// Extension methods for Product to help with BOM and unit management
/// </summary>
public static class ProductExtensions
{
    /// <summary>
    /// Gets the total cost of materials needed to produce one unit of this product
    /// </summary>
    public static decimal? GetMaterialCost(this Product product)
    {
        if (!product.IsManufactured || !product.Components.Any())
            return null;

        decimal totalCost = 0;
        foreach (var component in product.Components)
        {
            if (component.CostPerUnit.HasValue)
            {
                totalCost += component.QuantityRequired * component.CostPerUnit.Value;
            }
        }

        return totalCost;
    }

    /// <summary>
    /// Checks if sufficient raw materials are available for production
    /// </summary>
    public static bool CanProduce(this Product product, decimal quantityToProduce, Func<int, decimal> getAvailableStock)
    {
        if (!product.IsManufactured)
            return true;

        foreach (var component in product.Components.Where(c => c.IsCritical))
        {
            var requiredQuantity = component.QuantityRequired * quantityToProduce;
            var availableQuantity = getAvailableStock(component.ComponentProductId);

            if (availableQuantity < requiredQuantity)
                return false;
        }

        return true;
    }

    /// <summary>
    /// Gets a human-readable display name for the product
    /// </summary>
    public static string GetDisplayName(this Product product, string language = "en")
    {
        var name = language.ToLower() == "fa" ? product.NameFa : product.NameEn;
        var brand = language.ToLower() == "fa" ? product.BrandFa : product.BrandEn;

        return string.IsNullOrEmpty(brand) ? name : $"{brand} {name}";
    }
}
