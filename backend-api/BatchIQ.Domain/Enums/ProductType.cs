using System;

namespace BatchIQ.Domain.Enums;

public enum ProductType
{
    RawMaterial = 1,    // Basic ingredients (Pistachio, Almond, etc.)
    SemiFinished = 2,   // Intermediate products 
    Finished = 3,       // Final products ready for sale
    Service = 4         // Non-physical products
}
