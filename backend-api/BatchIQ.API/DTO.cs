using System;
using BatchIQ.Domain.Enums;

namespace BatchIQ.API;

internal record ProductDto(string PersianName,string Brand,decimal Size,SizeUnit SizeUnit);
internal record TransactionDto(int ProductId,int? FromLocationId,int? ToLocationId,
                               decimal Quantity,TransactionType TransactionType,
                               DateTime? ExpiryDate);