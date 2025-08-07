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
            SizeUnit.gr => quantity,
            SizeUnit.kg => quantity * 1000m,
            SizeUnit.lb => quantity * 453.592m,
            SizeUnit.ea => quantity, // Each is already base unit for count
            SizeUnit.pkg => quantity, // Package is already base unit for count
            SizeUnit.plb => quantity * 453.592m, // Pounds (lbs) to grams
            SizeUnit.phandered => quantity * 100, // Hundreds to grams
            // Count conversions (base: pieces)
            SizeUnit.piece => quantity,
            SizeUnit.pack => quantity, // Pack is already base unit for count
            SizeUnit.box => quantity, // Box is already base unit for count 


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
            SizeUnit.gr => baseQuantity,
            SizeUnit.kg => baseQuantity / 1000m,
            SizeUnit.lb => baseQuantity / 453.592m,
            SizeUnit.ea => baseQuantity, // Each is already base unit for count
            SizeUnit.pkg => baseQuantity, // Package is already base unit for count
            SizeUnit.plb => baseQuantity / 453.592m, // Pounds (lbs) from grams
            SizeUnit.phandered => baseQuantity / 100, // Hundreds from


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
            SizeUnit.gr or SizeUnit.kg or SizeUnit.lb => UnitCategory.Weight,
            SizeUnit.piece or SizeUnit.box or SizeUnit.ea or SizeUnit.pack => UnitCategory.Count,
            SizeUnit.ml or SizeUnit.l => UnitCategory.Volume,
            SizeUnit.pkg or SizeUnit.plb or SizeUnit.phandered => UnitCategory.Count, // Treat these as count for simplicity

            SizeUnit.other => UnitCategory.Count, // Treat 'other' as count for flexibility
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
            UnitCategory.Weight => SizeUnit.gr,
            UnitCategory.Count => SizeUnit.ea,
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
