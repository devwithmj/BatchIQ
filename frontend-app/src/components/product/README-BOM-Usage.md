# Enhanced BOMViewDialog Usage Guide

## Overview
The BOMViewDialog component has been enhanced to support BatchIQ's dual manufacturing paradigms:
- **Traditional BOM Manufacturing** (Fixed recipes like Mixed Nuts)
- **Process Manufacturing** (Variable yields like Pistachio Roasting)

## Updated Component Props

```tsx
interface Props {
  productId: number;
  productName: string;
  productNameFa?: string;           // Farsi name for bilingual support
  isManufactured?: boolean;         // Traditional BOM manufacturing
  isProcessedProduct?: boolean;     // Process manufacturing (future)
  productType?: ProductType;        // Raw Material, Ingredient, etc.
  children?: React.ReactNode;
}
```

## Usage Examples

### Traditional BOM Product (Mixed Nuts)
```tsx
<BOMViewDialog
  productId={product.id}
  productName={product.nameEn}
  productNameFa={product.nameFa}
  isManufactured={true}
  isProcessedProduct={false}
  productType={ProductType.FinishedProduct}
>
  <Button>View BOM</Button>
</BOMViewDialog>
```

### Process Manufacturing Product (Roasted Pistachios)
```tsx
<BOMViewDialog
  productId={product.id}
  productName={product.nameEn}
  productNameFa={product.nameFa}
  isManufactured={false}
  isProcessedProduct={true}
  productType={ProductType.SemiFinished}
>
  <Button>View Production</Button>
</BOMViewDialog>
```

### Hybrid Product (Uses both BOM and Process)
```tsx
<BOMViewDialog
  productId={product.id}
  productName={product.nameEn}
  productNameFa={product.nameFa}
  isManufactured={true}
  isProcessedProduct={true}
  productType={ProductType.FinishedProduct}
>
  <Button>View Manufacturing</Button>
</BOMViewDialog>
```

## Enhanced Features

### 1. Manufacturing Type Indicators
- **Factory Icon**: BOM Manufacturing
- **Workflow Icon**: Process Manufacturing
- **Type Badges**: Clear visual identification

### 2. Component Classification
- Shows component product types (Raw Material, Ingredient, etc.)
- Indicates if components are manufactured or processed
- Visual badges for easy identification

### 3. Enhanced BOM Table
- Added "Type" column for component classification
- Manufacturing method indicators (BOM/Process)
- Improved visual hierarchy

### 4. Summary Statistics
- Total components count
- Critical components count
- Material cost summary
- Process batch preview (coming soon)

### 5. Future Process Manufacturing Integration
- Tab interface for BOM vs Process views
- Placeholder for production batch management
- Yield tracking preparation

## Integration with Inventory Page

Update your inventory page to pass the new props:

```tsx
// In inventory/page.tsx
{products.map((product) => (
  <BOMViewDialog
    key={product.id}
    productId={product.id}
    productName={product.nameEn}
    productNameFa={product.nameFa}
    isManufactured={product.isManufactured}
    isProcessedProduct={product.isProcessedProduct}
    productType={product.productType}
  >
    <Button variant="outline" size="sm">
      <Eye className="h-4 w-4" />
    </Button>
  </BOMViewDialog>
))}
```

## Backend API Requirements

### Current BOM API (Implemented)
- `GET /api/product-bom/product/{productId}` - Get BOM items
- `GET /api/product-bom/material-cost/{productId}` - Get material cost
- `POST /api/product-bom` - Create BOM item
- `PUT /api/product-bom/{id}` - Update BOM item
- `DELETE /api/product-bom/{id}` - Delete BOM item

### Future Process Manufacturing APIs (To Be Implemented)
```
POST   /api/production-batches
GET    /api/production-batches/{id}
GET    /api/production-batches/by-product/{productId}
POST   /api/process-templates
GET    /api/process-templates/by-type/{processType}
```

### Enhanced Product Schema (Recommended)
The Product API should include these new fields:
```json
{
  "id": 1,
  "nameEn": "Premium Mixed Nuts",
  "nameFa": "آجیل مخلوط درجه یک",
  "productType": 3,
  "isManufactured": true,
  "isProcessedProduct": false,
  "unitType": 2,
  "baseUnit": 1
}
```

## Error Handling

The component includes robust error handling for:
- API response validation
- Array type checking
- Null/undefined data
- Network failures
- Invalid product types

## Performance Considerations

- Queries are enabled only when dialog is open
- Lazy loading of BOM data
- Efficient re-rendering with React Query
- Optimistic updates for mutations

## Future Enhancements

1. **Process Manufacturing Tab**: Full process batch management
2. **Yield Tracking**: Visual yield charts and trends
3. **Cost Allocation**: Dynamic cost distribution for process outputs
4. **Recipe Optimization**: AI-driven recipe suggestions
5. **Multi-level BOM**: Nested component hierarchies
6. **Version Control**: BOM revision tracking

## Testing Scenarios

1. **Empty BOM**: Product with no components
2. **Mixed Components**: Raw materials + manufactured components
3. **Critical Path**: Products with critical components
4. **Cost Calculation**: Accurate material cost aggregation
5. **Bilingual Content**: English + Farsi product names
6. **Error States**: Network failures and invalid data
7. **Process Products**: Future process manufacturing integration

---

**Note**: This component is prepared for your dual manufacturing system and will seamlessly integrate with process manufacturing APIs when they're implemented.
