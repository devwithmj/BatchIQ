"use client";

import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import {
  Form,
  FormField,
  FormItem,
  FormLabel,
  FormControl,
  FormMessage,
} from "@/components/ui/form";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectTrigger,
  SelectValue,
  SelectContent,
  SelectItem,
} from "@/components/ui/select";
import { Button } from "@/components/ui/button";

import { useForm } from "react-hook-form";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";

import { api } from "@/lib/api";
import { 
  SizeUnit,
  sizeUnitLabels,
  BOMItem
} from "@/lib/bom-schema";
import { Product } from "@/lib/product-schema";
import { Pencil, Plus } from "lucide-react";
import { toast } from "sonner";

// Simplified schemas for this component
const createBOMSchema = z.object({
  parentProductId: z.number(),
  componentProductId: z.number(),
  quantityRequired: z.coerce.number().positive(),
  unit: z.number(),
  costPerUnit: z.coerce.number().positive().optional(),
  isCritical: z.boolean().default(false),
  notes: z.string().optional(),
  sequence: z.coerce.number().int().optional(),
});

const updateBOMSchema = z.object({
  quantityRequired: z.coerce.number().positive(),
  unit: z.number(),
  costPerUnit: z.coerce.number().positive().optional(),
  isCritical: z.boolean().default(false),
  notes: z.string().optional(),
  sequence: z.coerce.number().int().optional(),
});

type Props = {
  parentProductId: number;
  defaultValues?: BOMItem;
  children?: React.ReactNode;
};

export default function BOMFormDialog({ parentProductId, defaultValues, children }: Props) {
  const isEdit = !!defaultValues?.id;
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();

  // Fetch available component products
  const { data: availableComponents = [] } = useQuery<Product[]>({
    queryKey: ["available-components", parentProductId],
    queryFn: async () => (await api.get(`/api/product-bom/available-components/${parentProductId}`)).data,
    enabled: !isEdit, // Only needed for new components
  });

  const form = useForm<any>({
    defaultValues: isEdit ? {
      quantityRequired: defaultValues.quantityRequired,
      unit: defaultValues.unit,
      costPerUnit: defaultValues.costPerUnit,
      isCritical: defaultValues.isCritical,
      notes: defaultValues.notes || "",
      sequence: defaultValues.sequence,
    } : {
      parentProductId,
      componentProductId: 0,
      quantityRequired: 0,
      unit: SizeUnit.Gram,
      costPerUnit: 0,
      isCritical: false,
      notes: "",
      sequence: 0,
    },
  });

  const mutation = useMutation({
    mutationFn: async (data: any) => {
      if (isEdit) {
        await api.put(`/api/product-bom/${defaultValues!.id}`, data);
      } else {
        await api.post("/api/product-bom", data);
      }
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["product-bom"] });
      qc.invalidateQueries({ queryKey: ["products"] });
      qc.invalidateQueries({ queryKey: ["available-components"] });
      toast.success("BOM item saved ✅");
      setOpen(false);
    },
    onError: (error) => {
      console.error("Error saving BOM item:", error);
      toast.error("Failed to save BOM item ❌");
    },
  });

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {children ?? (
          <Button variant={isEdit ? "ghost" : "default"} size="sm">
            {isEdit ? (
              <Pencil className="h-4 w-4" />
            ) : (
              <Plus className="h-4 w-4" />
            )}
            <span className="sr-only">{isEdit ? "Edit" : "Add"} BOM Item</span>
          </Button>
        )}
      </DialogTrigger>

      <DialogContent className="max-w-2xl">
        <DialogHeader>
          <DialogTitle className="text-lg font-medium">
            {isEdit ? "Edit BOM Component" : "Add BOM Component"}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form
            className="grid gap-4"
            onSubmit={form.handleSubmit((data) => mutation.mutate(data))}
          >
            {/* Component Product Selection (only for new items) */}
            {!isEdit && (
              <FormField
                control={form.control}
                name="componentProductId"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Component Product</FormLabel>
                    <FormControl>
                      <Select
                        value={field.value?.toString()}
                        onValueChange={(value) => field.onChange(parseInt(value))}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select a component product" />
                        </SelectTrigger>
                        <SelectContent>
                          {availableComponents.map((product) => (
                            <SelectItem key={product.id} value={product.id.toString()}>
                              <div>
                                <div className="font-medium">{product.nameEn}</div>
                                <div className="text-sm text-muted-foreground">
                                  {product.brandEn} - {product.nameFa}
                                </div>
                              </div>
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            )}

            <div className="grid grid-cols-2 gap-4">
              {/* Quantity Required */}
              <FormField
                control={form.control}
                name="quantityRequired"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Quantity Required</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        step="0.01"
                        placeholder="1.5"
                        value={field.value || ""}
                        onChange={(e) => field.onChange(e.target.value)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Unit */}
              <FormField
                control={form.control}
                name="unit"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Unit</FormLabel>
                    <FormControl>
                      <Select
                        value={field.value?.toString()}
                        onValueChange={(value) => field.onChange(parseInt(value))}
                      >
                        <SelectTrigger>
                          <SelectValue placeholder="Select unit" />
                        </SelectTrigger>
                        <SelectContent>
                          {Object.entries(sizeUnitLabels).map(([value, label]) => (
                            <SelectItem key={value} value={value}>
                              {label}
                            </SelectItem>
                          ))}
                        </SelectContent>
                      </Select>
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            <div className="grid grid-cols-2 gap-4">
              {/* Cost Per Unit */}
              <FormField
                control={form.control}
                name="costPerUnit"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Cost Per Unit (Optional)</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        step="0.01"
                        placeholder="0.00"
                        value={field.value || ""}
                        onChange={(e) => field.onChange(e.target.value)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />

              {/* Sequence */}
              <FormField
                control={form.control}
                name="sequence"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Sequence (Optional)</FormLabel>
                    <FormControl>
                      <Input
                        type="number"
                        placeholder="1"
                        value={field.value || ""}
                        onChange={(e) => field.onChange(e.target.value)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Critical checkbox */}
            <FormField
              control={form.control}
              name="isCritical"
              render={({ field }) => (
                <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                  <FormControl>
                    <input
                      type="checkbox"
                      checked={field.value}
                      onChange={(e) => field.onChange(e.target.checked)}
                      className="mt-1"
                    />
                  </FormControl>
                  <div className="space-y-1 leading-none">
                    <FormLabel>Critical Component</FormLabel>
                    <p className="text-sm text-muted-foreground">
                      Mark as critical if this component is essential for production
                    </p>
                  </div>
                </FormItem>
              )}
            />

            {/* Notes */}
            <FormField
              control={form.control}
              name="notes"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Notes (Optional)</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="Additional notes about this component..."
                      value={field.value || ""}
                      onChange={(e) => field.onChange(e.target.value)}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Submit button */}
            <div className="flex justify-end gap-2 pt-4">
              <Button
                type="button"
                variant="outline"
                onClick={() => setOpen(false)}
              >
                Cancel
              </Button>
              <Button type="submit" disabled={mutation.isPending}>
                {mutation.isPending ? "Saving..." : "Save"}
              </Button>
            </div>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
