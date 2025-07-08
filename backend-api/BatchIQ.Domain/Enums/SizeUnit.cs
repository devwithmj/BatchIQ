using System;

namespace BatchIQ.Domain.Enums;

public enum SizeUnit 
{ 
    // Weight units
    g = 1,      // gram (base unit for weight)
    kg = 2,     // kilogram
    lb = 3,     // pound
    oz = 4,     // ounce
    
    // Count units
    Piece = 10, // individual items (base unit for count)
    Dozen = 11, // 12 pieces
    Case = 12,  // variable count per case
    
    // Volume units (if needed)
    ml = 20,    // milliliter (base unit for volume)
    l = 21,     // liter
    gal = 22    // gallon
}
