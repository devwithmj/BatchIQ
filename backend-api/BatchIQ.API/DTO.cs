using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.API;

internal record ProductDto(int? id,
    string NameEn,
    string NameFa,
    string BrandEn,
    string BrandFa,
    decimal SizeValue,
    SizeUnit UnitType,
    decimal Price,
    List<string> Codes
);

// Enhanced Product DTOs
internal record CreateProductDto(
    string NameEn,
    string NameFa,
    string BrandEn,
    string BrandFa,
    ProductType? ProductType,
    decimal SizeValue,
    SizeUnit UnitType,
    SizeUnit? BaseUnit,
    decimal Price,
    bool? IsManufactured,
    List<string>? Codes
);

internal record UpdateProductDto(
    string NameEn,
    string NameFa,
    string BrandEn,
    string BrandFa,
    ProductType? ProductType,
    decimal SizeValue,
    SizeUnit UnitType,
    SizeUnit? BaseUnit,
    decimal Price,
    bool? IsManufactured,
    List<string>? Codes
);

// Enhanced Product DTO with BOM information
internal record ProductWithBOMDto(
    int Id,
    string NameEn,
    string NameFa,
    string BrandEn,
    string BrandFa,
    ProductType ProductType,
    decimal SizeValue,
    SizeUnit UnitType,
    SizeUnit BaseUnit,
    decimal Price,
    bool IsManufactured,
    List<string> Codes,
    List<ProductBOMDto> Components,
    decimal? MaterialCost
);

// ProductBOM DTOs
internal record ProductBOMDto(
    int Id,
    int ParentProductId,
    string ParentProductName,
    int ComponentProductId,
    string ComponentProductName,
    decimal QuantityRequired,
    SizeUnit Unit,
    decimal? CostPerUnit,
    bool IsCritical,
    string? Notes,
    int? Sequence
);

internal record CreateProductBOMDto(
    int ParentProductId,
    int ComponentProductId,
    decimal QuantityRequired,
    SizeUnit Unit,
    decimal? CostPerUnit = null,
    bool IsCritical = true,
    string? Notes = null,
    int? Sequence = null
);

internal record UpdateProductBOMDto(
    decimal QuantityRequired,
    SizeUnit Unit,
    decimal? CostPerUnit = null,
    bool IsCritical = true,
    string? Notes = null,
    int? Sequence = null
);

// Original DTOs
internal record LocationDto(
    int? Id,
    string Name,
    LocationType LocationType,
    int? ParentLocationId
);

internal record TransactionDto(int ProductId, int? FromLocationId, int? ToLocationId,
                               decimal Quantity, SizeUnit Unit, TransactionType TransactionType,
                               DateTime? ExpiryDate, string? BatchNumber, 
                               string? Notes, decimal? UnitCost);

// External Price Update DTOs
internal record ExternalPriceUpdateDto(
    string ProductCode,
    decimal NewPrice,
    string? Notes = null
);

internal record ExternalPriceUpdateResponseDto(
    bool Success,
    string Message,
    string ProductCode,
    decimal? OldPrice,
    decimal? NewPrice,
    bool IsNewProduct
);

// Clear External Tracking DTOs
internal record ClearExternalTrackingDto(
    List<int>? ProductIds = null,
    DateTime? UpdatedSince = null
);