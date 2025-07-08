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
}
