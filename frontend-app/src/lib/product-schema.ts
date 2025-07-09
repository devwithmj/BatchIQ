import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { SizeUnit, ProductType } from "./bom-schema";

export const productSchema = z.object({
  nameEn: z.string().min(2),
  nameFa: z.string().min(2),
  brandEn: z.string().min(1),
  brandFa: z.string().min(1),
  productType: z.nativeEnum(ProductType),
  sizeValue: z.coerce.number().positive(),
  unitType: z.nativeEnum(SizeUnit),
  baseUnit: z.nativeEnum(SizeUnit),
  price: z.coerce.number().positive(),
  isManufactured: z.boolean(),
  isProcessedProduct: z.boolean().optional(),
  codes: z.array(z.string().min(1)).optional(),
});

export type ProductFormValues = z.infer<typeof productSchema>;
export const resolver = zodResolver(productSchema);

// Product type for display
export type Product = {
  id: number;
  nameEn: string;
  nameFa: string;
  brandEn: string;
  brandFa: string;
  productType: typeof ProductType[keyof typeof ProductType];
  sizeValue: number;
  unitType: typeof SizeUnit[keyof typeof SizeUnit];
  baseUnit: typeof SizeUnit[keyof typeof SizeUnit];
  price: number;
  isManufactured: boolean;
  isProcessedProduct?: boolean; // New field for process manufacturing
  codes: string[];
};
