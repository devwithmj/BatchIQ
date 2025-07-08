using BatchIQ.Domain.Entities;

namespace BatchIQ.API.Models;

public static class ResponseModels
{
    public static object ToProductResponse(this Product product)
    {
        return new
        {
            id = product.Id,
            nameEn = product.NameEn,
            nameFa = product.NameFa,
            brandEn = product.BrandEn,
            brandFa = product.BrandFa,
            sizeValue = product.SizeValue,
            unitType = product.UnitType,
            price = product.Price,
            codes = product.Codes?.Select(c => c.Code).ToList() ?? new List<string>()
        };
    }

    public static object ToLocationResponse(this Location location)
    {
        return new
        {
            id = location.Id,
            name = location.Name,
            locationType = location.LocationType,
            parentLocationId = location.ParentLocationId,
            parentLocationName = location.ParentLocation?.Name,
            childrenCount = location.Children?.Count ?? 0
        };
    }

    public static object ToLocationDetailResponse(this Location location)
    {
        return new
        {
            id = location.Id,
            name = location.Name,
            locationType = location.LocationType,
            parentLocationId = location.ParentLocationId,
            parentLocationName = location.ParentLocation?.Name,
            children = location.Children?.Select(c => (object)new
            {
                id = c.Id,
                name = c.Name,
                locationType = c.LocationType
            })?.ToList() ?? new List<object>()
        };
    }

    public static object ToLocationCreatedResponse(this Location location)
    {
        return new
        {
            id = location.Id,
            name = location.Name,
            locationType = location.LocationType,
            parentLocationId = location.ParentLocationId
        };
    }

    public static object ToTransactionResponse(this InventoryTransaction transaction)
    {
        return new
        {
            id = transaction.Id,
            productId = transaction.ProductId,
            productNameEn = transaction.Product?.NameEn,
            productNameFa = transaction.Product?.NameFa,
            fromLocationId = transaction.FromLocationId,
            fromLocationName = transaction.FromLocation?.Name,
            toLocationId = transaction.ToLocationId,
            toLocationName = transaction.ToLocation?.Name,
            quantity = transaction.Quantity,
            transactionType = transaction.TransactionType,
            expiryDate = transaction.ExpiryDate,
            createdAt = transaction.Id // You may want to add a CreatedAt property to the entity
        };
    }

    public static object ToTransactionDetailResponse(this InventoryTransaction transaction)
    {
        return new
        {
            id = transaction.Id,
            productId = transaction.ProductId,
            product = transaction.Product != null ? new
            {
                id = transaction.Product.Id,
                nameEn = transaction.Product.NameEn,
                nameFa = transaction.Product.NameFa,
                brandEn = transaction.Product.BrandEn,
                brandFa = transaction.Product.BrandFa
            } : null,
            fromLocationId = transaction.FromLocationId,
            fromLocation = transaction.FromLocation != null ? new
            {
                id = transaction.FromLocation.Id,
                name = transaction.FromLocation.Name,
                locationType = transaction.FromLocation.LocationType
            } : null,
            toLocationId = transaction.ToLocationId,
            toLocation = transaction.ToLocation != null ? new
            {
                id = transaction.ToLocation.Id,
                name = transaction.ToLocation.Name,
                locationType = transaction.ToLocation.LocationType
            } : null,
            quantity = transaction.Quantity,
            transactionType = transaction.TransactionType,
            expiryDate = transaction.ExpiryDate
        };
    }

    public static object ToTransactionCreatedResponse(this InventoryTransaction transaction)
    {
        return new
        {
            id = transaction.Id,
            productId = transaction.ProductId,
            fromLocationId = transaction.FromLocationId,
            toLocationId = transaction.ToLocationId,
            quantity = transaction.Quantity,
            transactionType = transaction.TransactionType,
            expiryDate = transaction.ExpiryDate
        };
    }
}
