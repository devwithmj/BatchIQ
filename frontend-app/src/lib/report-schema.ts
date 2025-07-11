import * as z from "zod";

// Enum schemas
export const TransactionTypeSchema = z.enum([
  "IN",
  "OUT", 
  "TRANSFER",
  "ADJUSTMENT",
  "MANUFACTURING_IN",
  "MANUFACTURING_OUT"
]);

export type TransactionType = z.infer<typeof TransactionTypeSchema>;

// Location Stock DTO
export const LocationStockSchema = z.object({
  locationId: z.number(),
  locationName: z.string().nullable(),
  stock: z.number(),
  unit: z.string().nullable(),
});

export type LocationStock = z.infer<typeof LocationStockSchema>;

// Stock Availability Report DTO
export const StockAvailabilityReportSchema = z.object({
  productId: z.number(),
  productNameEn: z.string().nullable(),
  productNameFa: z.string().nullable(),
  brandEn: z.string().nullable(),
  brandFa: z.string().nullable(),
  currentStock: z.number(),
  baseUnit: z.string().nullable(),
  minimumStock: z.number().nullable(),
  reorderPoint: z.number().nullable(),
  isLowStock: z.boolean(),
  needsReorder: z.boolean(),
  lastTransaction: z.string().datetime(),
  locationBreakdown: z.array(LocationStockSchema).nullable(),
});

export type StockAvailabilityReport = z.infer<typeof StockAvailabilityReportSchema>;

// Product Transaction Report DTO
export const ProductTransactionReportSchema = z.object({
  transactionId: z.number(),
  productId: z.number(),
  productNameEn: z.string().nullable(),
  productNameFa: z.string().nullable(),
  fromLocationName: z.string().nullable(),
  toLocationName: z.string().nullable(),
  quantity: z.number(),
  unit: z.string().nullable(),
  baseQuantity: z.number(),
  baseUnit: z.string().nullable(),
  transactionType: z.string().nullable(),
  timestamp: z.string().datetime(),
  batchNumber: z.string().nullable(),
  expiryDate: z.string().datetime().nullable(),
  notes: z.string().nullable(),
});

export type ProductTransactionReport = z.infer<typeof ProductTransactionReportSchema>;

// Transaction Summary DTO
export const TransactionSummarySchema = z.object({
  productId: z.number(),
  productNameEn: z.string().nullable(),
  productNameFa: z.string().nullable(),
  totalIn: z.number(),
  totalOut: z.number(),
  netStock: z.number(),
  baseUnit: z.string().nullable(),
  transactionCount: z.number(),
  firstTransaction: z.string().datetime().nullable(),
  lastTransaction: z.string().datetime().nullable(),
});

export type TransactionSummary = z.infer<typeof TransactionSummarySchema>;

// Query parameter schemas for API calls
export const StockAvailabilityQuerySchema = z.object({
  lowStockOnly: z.boolean().optional(),
  needsReorderOnly: z.boolean().optional(),
});

export type StockAvailabilityQuery = z.infer<typeof StockAvailabilityQuerySchema>;

export const ProductTransactionQuerySchema = z.object({
  fromDate: z.string().datetime().optional(),
  toDate: z.string().datetime().optional(),
  transactionType: z.string().optional(),
  locationId: z.number().optional(),
});

export type ProductTransactionQuery = z.infer<typeof ProductTransactionQuerySchema>;

export const TransactionSummaryQuerySchema = z.object({
  fromDate: z.string().datetime().optional(),
  toDate: z.string().datetime().optional(),
});

export type TransactionSummaryQuery = z.infer<typeof TransactionSummaryQuerySchema>;
