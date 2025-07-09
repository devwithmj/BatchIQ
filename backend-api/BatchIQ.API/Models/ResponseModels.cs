using BatchIQ.Domain.Entities;
using BatchIQ.Domain.Extensions;

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
            productType = product.ProductType,
            sizeValue = product.SizeValue,
            unitType = product.UnitType,
            baseUnit = product.BaseUnit,
            price = product.Price,
            isManufactured = product.IsManufactured,
            codes = product.Codes?.Select(c => c.Code).ToList() ?? new List<string>()
        };
    }

    public static object ToProductWithBOMResponse(this Product product)
    {
        return new
        {
            id = product.Id,
            nameEn = product.NameEn,
            nameFa = product.NameFa,
            brandEn = product.BrandEn,
            brandFa = product.BrandFa,
            productType = product.ProductType,
            sizeValue = product.SizeValue,
            unitType = product.UnitType,
            baseUnit = product.BaseUnit,
            price = product.Price,
            isManufactured = product.IsManufactured,
            materialCost = product.GetMaterialCost(),
            codes = product.Codes?.Select(c => c.Code).ToList() ?? new List<string>(),
            components = product.Components?.Select(c => c.ToProductBOMResponse()).ToList() ?? new List<object>(),
            usedInProducts = product.UsedInProducts?.Select(u => (object)new
            {
                id = u.Id,
                parentProductId = u.ParentProductId,
                parentProductName = u.ParentProduct?.NameEn,
                quantityRequired = u.QuantityRequired,
                unit = u.Unit
            }).ToList() ?? new List<object>()
        };
    }

    public static object ToProductBOMResponse(this ProductBOM bom)
    {
        return new
        {
            id = bom.Id,
            parentProductId = bom.ParentProductId,
            parentProductName = bom.ParentProduct?.NameEn,
            componentProductId = bom.ComponentProductId,
            componentProductName = bom.ComponentProduct?.NameEn,
            quantityRequired = bom.QuantityRequired,
            unit = bom.Unit,
            costPerUnit = bom.CostPerUnit,
            isCritical = bom.IsCritical,
            notes = bom.Notes,
            sequence = bom.Sequence,
            totalCost = bom.CostPerUnit.HasValue ? bom.QuantityRequired * bom.CostPerUnit.Value : (decimal?)null
        };
    }

    public static object ToProductBOMDetailResponse(this ProductBOM bom)
    {
        return new
        {
            id = bom.Id,
            parentProduct = bom.ParentProduct != null ? new
            {
                id = bom.ParentProduct.Id,
                nameEn = bom.ParentProduct.NameEn,
                nameFa = bom.ParentProduct.NameFa,
                brandEn = bom.ParentProduct.BrandEn,
                brandFa = bom.ParentProduct.BrandFa,
                baseUnit = bom.ParentProduct.BaseUnit
            } : null,
            componentProduct = bom.ComponentProduct != null ? new
            {
                id = bom.ComponentProduct.Id,
                nameEn = bom.ComponentProduct.NameEn,
                nameFa = bom.ComponentProduct.NameFa,
                brandEn = bom.ComponentProduct.BrandEn,
                brandFa = bom.ComponentProduct.BrandFa,
                baseUnit = bom.ComponentProduct.BaseUnit,
                currentPrice = bom.ComponentProduct.Price
            } : null,
            quantityRequired = bom.QuantityRequired,
            unit = bom.Unit,
            costPerUnit = bom.CostPerUnit,
            isCritical = bom.IsCritical,
            notes = bom.Notes,
            sequence = bom.Sequence,
            totalCost = bom.CostPerUnit.HasValue ? bom.QuantityRequired * bom.CostPerUnit.Value : (decimal?)null
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

    // Authentication Response Models
    public static object ToUserResponse(this User user)
    {
        return new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            firstNameFa = user.FirstNameFa,
            lastNameFa = user.LastNameFa,
            isActive = user.IsActive,
            emailConfirmed = user.EmailConfirmed,
            createdAt = user.CreatedAt,
            lastLoginAt = user.LastLoginAt,
            roles = user.UserRoles?.Select(ur => ur.Role.ToRoleResponse()).ToList() ?? new List<object>()
        };
    }

    public static object ToUserDetailResponse(this User user)
    {
        return new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName,
            firstNameFa = user.FirstNameFa,
            lastNameFa = user.LastNameFa,
            isActive = user.IsActive,
            emailConfirmed = user.EmailConfirmed,
            createdAt = user.CreatedAt,
            updatedAt = user.UpdatedAt,
            lastLoginAt = user.LastLoginAt,
            roles = user.UserRoles?.Select(ur => ur.Role.ToRoleResponse()).ToList() ?? new List<object>()
        };
    }

    public static object ToRoleResponse(this Role role)
    {
        return new
        {
            id = role.Id,
            name = role.Name,
            description = role.Description,
            descriptionFa = role.DescriptionFa,
            isActive = role.IsActive,
            createdAt = role.CreatedAt,
            permissions = role.RolePermissions?.Select(rp => rp.Permission.ToPermissionResponse()).ToList() ?? new List<object>()
        };
    }

    public static object ToPermissionResponse(this Permission permission)
    {
        return new
        {
            id = permission.Id,
            name = permission.Name,
            description = permission.Description,
            descriptionFa = permission.DescriptionFa,
            category = permission.Category,
            isActive = permission.IsActive
        };
    }

    public static object ToLoginResponse(this User user, string token, string refreshToken, List<string> permissions)
    {
        return new
        {
            token = token,
            refreshToken = refreshToken,
            user = user.ToUserResponse(),
            roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList() ?? new List<string>(),
            permissions = permissions
        };
    }
}
