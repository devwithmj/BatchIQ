import { z } from "zod";

export const locationSchema = z.object({
  name: z.string().min(2, "Name must be at least 2 characters"),
  locationType: z.coerce.number().min(1).max(4),
  parentLocationId: z.coerce.number().optional().nullable(),
});

export type LocationFormValues = z.infer<typeof locationSchema>;

export type Location = {
  id: number;
  name: string;
  locationType: number;
  parentLocationId?: number | null;
  children?: Location[];
};

// Location types enum mapping based on backend enum (1-4)
export const LocationTypes = {
  1: "Warehouse",
  2: "Zone",
  3: "Aisle",
  4: "Shelf",
} as const;

export type LocationTypeLabel = keyof typeof LocationTypes;
