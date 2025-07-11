using System.ComponentModel.DataAnnotations;

namespace BatchIQ.API.Models;

public record UserDto(
    int Id,
    string Username,
    string Email,
    string FirstName,
    string LastName,
    string? FirstNameFa,
    string? LastNameFa,
    bool IsActive,
    bool EmailConfirmed,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    List<RoleDto> Roles
);

public record RoleDto(
    int Id,
    string Name,
    string? Description,
    string? DescriptionFa,
    bool IsActive,
    DateTime CreatedAt,
    List<PermissionDto> Permissions
);

public record PermissionDto(
    int Id,
    string Name,
    string? Description,
    string? DescriptionFa,
    string Category,
    bool IsActive
);

public record PermissionCategoryDto(
    string Category,
    List<PermissionDto> Permissions
);

public record CreateRoleRequest(
    string Name,
    string? Description,
    string? DescriptionFa,
    List<int> PermissionIds
);

public record UpdateRoleRequest(
    string Name,
    string? Description,
    string? DescriptionFa,
    List<int> PermissionIds
);

public record UpdatePermissionRequest(
    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Permission name must be between 1 and 100 characters")]
    string Name,
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    string? Description,
    
    [StringLength(200, ErrorMessage = "Farsi description cannot exceed 200 characters")]
    string? DescriptionFa,
    
    [Required(ErrorMessage = "Permission category is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 50 characters")]
    string Category,
    
    bool IsActive
);

public record CreatePermissionRequest(
    [Required(ErrorMessage = "Permission name is required")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Permission name must be between 1 and 100 characters")]
    string Name,
    
    [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters")]
    string? Description,
    
    [StringLength(200, ErrorMessage = "Farsi description cannot exceed 200 characters")]
    string? DescriptionFa,
    
    [Required(ErrorMessage = "Permission category is required")]
    [StringLength(50, MinimumLength = 1, ErrorMessage = "Category must be between 1 and 50 characters")]
    string Category
);

// Report DTOs
public record StockAvailabilityReportDto(
    int ProductId,
    string ProductNameEn,
    string ProductNameFa,
    string BrandEn,
    string BrandFa,
    decimal CurrentStock,
    string BaseUnit,
    decimal? MinimumStock,
    decimal? ReorderPoint,
    bool IsLowStock,
    bool NeedsReorder,
    DateTime LastTransaction,
    List<LocationStockDto> LocationBreakdown
);

public record LocationStockDto(
    int LocationId,
    string LocationName,
    decimal Stock,
    string Unit
);

public record ProductTransactionReportDto(
    int TransactionId,
    int ProductId,
    string ProductNameEn,
    string ProductNameFa,
    string? FromLocationName,
    string? ToLocationName,
    decimal Quantity,
    string Unit,
    decimal BaseQuantity,
    string BaseUnit,
    string TransactionType,
    DateTime Timestamp,
    string? BatchNumber,
    DateTime? ExpiryDate,
    string? Notes
);

public record TransactionSummaryDto(
    int ProductId,
    string ProductNameEn,
    string ProductNameFa,
    decimal TotalIn,
    decimal TotalOut,
    decimal NetStock,
    string BaseUnit,
    int TransactionCount,
    DateTime? FirstTransaction,
    DateTime? LastTransaction
);
