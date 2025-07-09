namespace BatchIQ.Domain.Enums;

public enum SystemRole
{
    SuperAdmin = 1,
    Admin = 2,
    Manager = 3,
    Operator = 4,
    Viewer = 5
}

public enum SystemPermission
{
    // Products Management
    ProductsView = 1,
    ProductsCreate = 2,
    ProductsEdit = 3,
    ProductsDelete = 4,
    
    // BOM Management
    BOMView = 5,
    BOMCreate = 6,
    BOMEdit = 7,
    BOMDelete = 8,
    
    // Inventory Management
    InventoryView = 9,
    InventoryCreate = 10,
    InventoryEdit = 11,
    InventoryDelete = 12,
    
    // Manufacturing Management
    ManufacturingView = 13,
    ManufacturingCreate = 14,
    ManufacturingEdit = 15,
    ManufacturingDelete = 16,
    
    // Production Batches
    ProductionBatchView = 17,
    ProductionBatchCreate = 18,
    ProductionBatchEdit = 19,
    ProductionBatchDelete = 20,
    
    // Location Management
    LocationView = 21,
    LocationCreate = 22,
    LocationEdit = 23,
    LocationDelete = 24,
    
    // User Management
    UserView = 25,
    UserCreate = 26,
    UserEdit = 27,
    UserDelete = 28,
    
    // Role Management
    RoleView = 29,
    RoleCreate = 30,
    RoleEdit = 31,
    RoleDelete = 32,
    
    // Reports
    ReportsView = 33,
    ReportsExport = 34,
    
    // System Administration
    SystemConfiguration = 35,
    SystemBackup = 36,
    SystemRestore = 37
}
