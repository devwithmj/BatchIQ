import { z } from "zod";

export const transactionSchema = z.object({
  productId: z.coerce.number().min(1, "Product is required"),
  fromLocationId: z.coerce.number().optional().nullable(),
  toLocationId: z.coerce.number().optional().nullable(),
  quantity: z.coerce.number().positive("Quantity must be positive"),
  transactionType: z.coerce.number().min(1).max(4),
  expiryDate: z.string().optional().nullable(),
}).refine((data) => {
  // Validation based on transaction type
  switch (data.transactionType) {
    case 1: // Stock In - requires toLocationId
      return data.toLocationId !== null && data.toLocationId !== undefined;
    case 2: // Stock Out - requires fromLocationId  
      return data.fromLocationId !== null && data.fromLocationId !== undefined;
    case 3: // Transfer - requires both locations
      return (data.fromLocationId !== null && data.fromLocationId !== undefined) &&
             (data.toLocationId !== null && data.toLocationId !== undefined) &&
             (data.fromLocationId !== data.toLocationId);
    case 4: // Adjustment - no location requirements
      return true;
    default:
      return true;
  }
}, {
  message: "Location requirements not met for selected transaction type",
});

export type TransactionFormValues = z.infer<typeof transactionSchema>;

export type Transaction = {
  id: number;
  productId: number;
  fromLocationId?: number | null;
  toLocationId?: number | null;
  quantity: number;
  transactionType: number;
  expiryDate?: string | null;
  createdAt?: number;
  // Fields returned by the API
  productNameEn?: string;
  productNameFa?: string;
  fromLocationName?: string | null;
  toLocationName?: string | null;
};

// Transaction types enum mapping based on backend enum (1-4)
export const TransactionTypes = {
  1: "Stock In",
  2: "Stock Out", 
  3: "Transfer",
  4: "Adjustment",
} as const;

export type TransactionTypeLabel = keyof typeof TransactionTypes;
