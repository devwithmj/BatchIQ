import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

// Enum mappings based on swagger
export const SizeUnit = {
  Gram: 1,
  Kilogram: 2,
  Pound: 3,
  Piece: 4,
  Liter: 10,
  Milliliter: 11,
  Gallon: 12,
  Meter: 20,
  Centimeter: 21,
  Inch: 22,
} as const;

export const sizeUnitLabels = {
  [SizeUnit.Gram]: "g",
  [SizeUnit.Kilogram]: "kg", 
  [SizeUnit.Pound]: "lb",
  [SizeUnit.Piece]: "pcs",
  [SizeUnit.Liter]: "L",
  [SizeUnit.Milliliter]: "mL",
  [SizeUnit.Gallon]: "gal",
  [SizeUnit.Meter]: "m",
  [SizeUnit.Centimeter]: "cm",
  [SizeUnit.Inch]: "in",
} as const;

export const ProductType = {
  RawMaterial: 1,
  Ingredient: 2,
  FinishedProduct: 3,
  Packaging: 4,
} as const;

export const productTypeLabels = {
  [ProductType.RawMaterial]: "Raw Material",
  [ProductType.Ingredient]: "Ingredient", 
  [ProductType.FinishedProduct]: "Finished Product",
  [ProductType.Packaging]: "Packaging",
} as const;

// BOM schemas
export const createBOMSchema = z.object({
  parentProductId: z.number(),
  componentProductId: z.number(),
  quantityRequired: z.coerce.number().positive(),
  unit: z.nativeEnum(SizeUnit),
  costPerUnit: z.coerce.number().positive().optional(),
  isCritical: z.boolean().default(false),
  notes: z.string().optional(),
  sequence: z.coerce.number().int().optional(),
});

export const updateBOMSchema = z.object({
  quantityRequired: z.coerce.number().positive(),
  unit: z.nativeEnum(SizeUnit),
  costPerUnit: z.coerce.number().positive().optional(),
  isCritical: z.boolean().default(false),
  notes: z.string().optional(),
  sequence: z.coerce.number().int().optional(),
});

export type CreateBOMFormValues = z.infer<typeof createBOMSchema>;
export type UpdateBOMFormValues = z.infer<typeof updateBOMSchema>;

export const createBOMResolver = zodResolver(createBOMSchema);
export const updateBOMResolver = zodResolver(updateBOMSchema);

// BOM types
export type BOMItem = {
  id: number;
  parentProductId: number;
  componentProductId: number;
  componentProduct: {
    id: number;
    nameEn: string;
    nameFa: string;
    brandEn: string;
    brandFa: string;
    unitType: number;
    price: number;
    productType?: number; // Enhanced for dual manufacturing
    isManufactured?: boolean;
    isProcessedProduct?: boolean;
  };
  quantityRequired: number;
  unit: number;
  costPerUnit?: number;
  isCritical: boolean;
  notes?: string;
  sequence?: number;
};

export type ProductWithBOM = {
  id: number;
  nameEn: string;
  nameFa: string;
  brandEn: string;
  brandFa: string;
  productType: number;
  sizeValue: number;
  unitType: number;
  baseUnit: number;
  price: number;
  isManufactured: boolean;
  codes: string[];
  billOfMaterials: BOMItem[];
};
