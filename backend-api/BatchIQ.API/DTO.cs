using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.API;

internal record ProductDto(
    string NameEn,
    string NameFa,
    string BrandEn,
    string BrandFa,
    decimal SizeValue,
    SizeUnit UnitType,
    decimal Price
);
internal record TransactionDto(int ProductId, int? FromLocationId, int? ToLocationId,
                               decimal Quantity, TransactionType TransactionType,
                               DateTime? ExpiryDate);