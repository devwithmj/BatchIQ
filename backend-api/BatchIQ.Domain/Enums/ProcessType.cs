using System;

namespace BatchIQ.Domain.Enums;

public enum ProcessType
{
    Assembly = 1,       // Traditional BOM assembly (Mixed Nuts)
    Roasting = 2,       // Roasting processes (Raw → Salted/Sour/Saffron)
    Mixing = 3,         // Mixing processes
    Packaging = 4,      // Packaging operations
    QualityControl = 5, // Quality control processes
    Seasoning = 6,      // Adding salt, sour, saffron
    Sorting = 7,        // Sorting by size/quality
    Cleaning = 8        // Cleaning raw materials
}
