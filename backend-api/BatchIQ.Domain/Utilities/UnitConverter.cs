using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.Domain.Utilities;

/// <summary>
/// Utility class for converting between different units of measurement
/// </summary>
public static class UnitConverter
{
    /// <summary>
    /// Convert quantity from one unit to another
    /// </summary>
    /// <param name="quantity">The quantity to convert</param>
    /// <param name="fromUnit">The original unit</param>
    /// <param name="toUnit">The target unit</param>
    /// <returns>The converted quantity</returns>
    /// <exception cref="ArgumentException">When units are incompatible</exception>
    public static decimal Convert(decimal quantity, SizeUnit fromUnit, SizeUnit toUnit)
    {
        if (fromUnit == toUnit)
            return quantity;

        // Convert to base unit first, then to target unit
        var baseQuantity = ConvertToBaseUnit(quantity, fromUnit);
        return ConvertFromBaseUnit(baseQuantity, toUnit, GetUnitCategory(fromUnit));
    }

    /// <summary>
    /// Convert quantity to the base unit for its category
    /// </summary>
    public static decimal ConvertToBaseUnit(decimal quantity, SizeUnit unit)
    {
        return unit switch
        {
            // Weight conversions (base: grams)
            SizeUnit.g => quantity,
            SizeUnit.kg => quantity * 1000m,
            SizeUnit.lb => quantity * 453.592m,
            SizeUnit.oz => quantity * 28.3495m,

            // Count conversions (base: pieces)
            SizeUnit.Piece => quantity,
            SizeUnit.Dozen => quantity * 12m,
            SizeUnit.Case => quantity, // Case conversion depends on product-specific configuration

            // Volume conversions (base: milliliters)
            SizeUnit.ml => quantity,
            SizeUnit.l => quantity * 1000m,
            SizeUnit.gal => quantity * 3785.41m,

            _ => throw new ArgumentException($"Unknown unit: {unit}")
        };
    }

    /// <summary>
    /// Convert from base unit to target unit
    /// </summary>
    public static decimal ConvertFromBaseUnit(decimal baseQuantity, SizeUnit targetUnit, UnitCategory category)
    {
        if (GetUnitCategory(targetUnit) != category)
            throw new ArgumentException($"Cannot convert between different unit categories");

        return targetUnit switch
        {
            // Weight conversions (from grams)
            SizeUnit.g => baseQuantity,
            SizeUnit.kg => baseQuantity / 1000m,
            SizeUnit.lb => baseQuantity / 453.592m,
            SizeUnit.oz => baseQuantity / 28.3495m,

            // Count conversions (from pieces)
            SizeUnit.Piece => baseQuantity,
            SizeUnit.Dozen => baseQuantity / 12m,
            SizeUnit.Case => baseQuantity, // Case conversion depends on product-specific configuration

            // Volume conversions (from milliliters)
            SizeUnit.ml => baseQuantity,
            SizeUnit.l => baseQuantity / 1000m,
            SizeUnit.gal => baseQuantity / 3785.41m,

            _ => throw new ArgumentException($"Unknown unit: {targetUnit}")
        };
    }

    /// <summary>
    /// Get the category of a unit (Weight, Count, Volume)
    /// </summary>
    public static UnitCategory GetUnitCategory(SizeUnit unit)
    {
        return unit switch
        {
            SizeUnit.g or SizeUnit.kg or SizeUnit.lb or SizeUnit.oz => UnitCategory.Weight,
            SizeUnit.Piece or SizeUnit.Dozen or SizeUnit.Case => UnitCategory.Count,
            SizeUnit.ml or SizeUnit.l or SizeUnit.gal => UnitCategory.Volume,
            _ => throw new ArgumentException($"Unknown unit: {unit}")
        };
    }

    /// <summary>
    /// Get the base unit for a unit category
    /// </summary>
    public static SizeUnit GetBaseUnit(UnitCategory category)
    {
        return category switch
        {
            UnitCategory.Weight => SizeUnit.g,
            UnitCategory.Count => SizeUnit.Piece,
            UnitCategory.Volume => SizeUnit.ml,
            _ => throw new ArgumentException($"Unknown category: {category}")
        };
    }
}

public enum UnitCategory
{
    Weight = 1,
    Count = 2,
    Volume = 3
}
