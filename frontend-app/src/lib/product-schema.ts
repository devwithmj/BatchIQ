import * as z from "zod";
import { zodResolver } from "@hookform/resolvers/zod";

export const productSchema = z.object({
  nameEn:  z.string().min(2),
  nameFa:  z.string().min(2),
  brandEn: z.string().min(1),
  brandFa: z.string().min(1),
  sizeValue: z.coerce.number().positive(),
  unitType:  z.enum(["g", "kg", "lb", "pcs"]),
  price:     z.coerce.number().positive(),
  codes:     z.array(z.object({ code: z.string().min(1) })).optional(),
});

export type ProductFormValues = z.infer<typeof productSchema>;
export const resolver = zodResolver(productSchema);
