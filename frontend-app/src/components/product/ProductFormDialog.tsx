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
import { Checkbox } from "@/components/ui/checkbox";

import { useForm } from "react-hook-form";
import { useMutation, useQueryClient } from "@tanstack/react-query";

import { api } from "@/lib/api";
import { ProductType, SizeUnit, productTypeLabels, sizeUnitLabels } from "@/lib/bom-schema";
import { ProductFormValues, resolver } from "@/lib/product-schema";
import { Pencil, Plus, Factory, Workflow } from "lucide-react";
import { toast } from "sonner";
import { TagsInput } from "../ui/taginput";

/* ------------------------------------------------------------------ */
/* 2️⃣  Dialog component                                               */
/* ------------------------------------------------------------------ */
type Props = {
  defaultValues?: Partial<ProductFormValues> & { id?: number };
  children?: React.ReactNode;
};

export default function ProductFormDialog({ defaultValues, children }: Props) {
  const isEdit = !!defaultValues?.id;
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();
  
  /* ---------- always supply a value so inputs start controlled ----- */
  const form = useForm<ProductFormValues>({
    resolver,
    defaultValues: {
      nameEn: defaultValues?.nameEn ?? "",
      nameFa: defaultValues?.nameFa ?? "",
      brandEn: defaultValues?.brandEn ?? "",
      brandFa: defaultValues?.brandFa ?? "",
      productType: defaultValues?.productType ?? ProductType.RawMaterial,
      sizeValue: defaultValues?.sizeValue ?? 0,
      unitType: defaultValues?.unitType ?? SizeUnit.Gram,
      baseUnit: defaultValues?.baseUnit ?? SizeUnit.Gram,
      price: defaultValues?.price ?? 0,
      isManufactured: defaultValues?.isManufactured ?? false,
      isProcessedProduct: defaultValues?.isProcessedProduct ?? false,
      codes: defaultValues?.codes ?? [],
    },
  });

  /* ---------------- mutation -------------------------------------- */
  const mutation = useMutation({
    mutationFn: async (data: ProductFormValues) => {
      if (isEdit) {
        await api.put(`/products/${defaultValues!.id}`, data);
      } else {
        await api.post("/products", data);
      }
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["inventory"] });
      toast.success("Saved ✅");
      setOpen(false);
    },
    onError: () => toast("Save Failed ❌"),
  });

  /* --------------------------- UI --------------------------------- */
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
            <span className="sr-only">{isEdit ? "Edit" : "Add"} Product</span>
          </Button>
        )}
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle className="text-lg font-medium">
            {isEdit ? "Edit Product" : "Add Product"}
          </DialogTitle>
        </DialogHeader>

        <Form {...form}>
          <form
            className="grid gap-4"
            onSubmit={form.handleSubmit((data) => mutation.mutate(data))}
          >
            {/* Names */}
            <FormField
              control={form.control}
              name="nameEn"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name (EN)</FormLabel>
                  <FormControl>
                    <Input placeholder="Fava Green Bean" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <FormField
              control={form.control}
              name="nameFa"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>نام محصول (FA)</FormLabel>
                  <FormControl>
                    <Input placeholder="لپه باقالا سبز" {...field} />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Brands */}
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="brandEn"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Brand (EN)</FormLabel>
                    <FormControl>
                      <Input placeholder="Pemina" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="brandFa"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>برند (FA)</FormLabel>
                    <FormControl>
                      <Input placeholder="پِمینا" {...field} />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Product Type */}
            <FormField
              control={form.control}
              name="productType"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Product Type</FormLabel>
                  <Select
                    onValueChange={(value) => field.onChange(parseInt(value))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(productTypeLabels).map(([value, label]) => (
                        <SelectItem key={value} value={value}>
                          {label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Size & Unit */}
            <div className="grid gap-4 md:grid-cols-2">
              <FormField
                control={form.control}
                name="sizeValue"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Size</FormLabel>
                    <FormControl>
                      <Input
                        placeholder="400"
                        type="number"
                        step="0.01"
                        {...field}
                        value={field.value || ""}
                        onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                      />
                    </FormControl>
                    <FormMessage />
                  </FormItem>
                )}
              />
              <FormField
                control={form.control}
                name="unitType"
                render={({ field }) => (
                  <FormItem>
                    <FormLabel>Unit</FormLabel>
                    <Select
                      onValueChange={(value) => field.onChange(parseInt(value))}
                      value={field.value?.toString()}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {Object.entries(sizeUnitLabels).map(([value, label]) => (
                          <SelectItem key={value} value={value}>
                            {label}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Base Unit */}
            <FormField
              control={form.control}
              name="baseUnit"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Base Unit</FormLabel>
                  <Select
                    onValueChange={(value) => field.onChange(parseInt(value))}
                    value={field.value?.toString()}
                  >
                    <FormControl>
                      <SelectTrigger>
                        <SelectValue />
                      </SelectTrigger>
                    </FormControl>
                    <SelectContent>
                      {Object.entries(sizeUnitLabels).map(([value, label]) => (
                        <SelectItem key={value} value={value}>
                          {label}
                        </SelectItem>
                      ))}
                    </SelectContent>
                  </Select>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Price */}
            <FormField
              control={form.control}
              name="price"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Price ($)</FormLabel>
                  <FormControl>
                    <Input
                      placeholder="4.49"
                      type="number"
                      step="0.01"
                      {...field}
                      value={field.value || ""}
                      onChange={(e) => field.onChange(parseFloat(e.target.value) || 0)}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Manufacturing Type - Group both checkboxes */}
            <div className="space-y-3">
              <FormField
                control={form.control}
                name="isManufactured"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                    <FormControl>
                      <Checkbox
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <div className="space-y-1 leading-none">
                      <FormLabel>
                        <Factory className="h-4 w-4 inline mr-1" />
                        BOM Manufacturing
                      </FormLabel>
                      <p className="text-xs text-muted-foreground">
                        Fixed recipe with defined components (e.g., Mixed Nuts)
                      </p>
                    </div>
                  </FormItem>
                )}
              />

              <FormField
                control={form.control}
                name="isProcessedProduct"
                render={({ field }) => (
                  <FormItem className="flex flex-row items-start space-x-3 space-y-0">
                    <FormControl>
                      <Checkbox
                        checked={field.value}
                        onCheckedChange={field.onChange}
                      />
                    </FormControl>
                    <div className="space-y-1 leading-none">
                      <FormLabel>
                        <Workflow className="h-4 w-4 inline mr-1" />
                        Process Manufacturing
                      </FormLabel>
                      <p className="text-xs text-muted-foreground">
                        Variable yield transformation (e.g., Roasted Pistachios)
                      </p>
                    </div>
                  </FormItem>
                )}
              />
            </div>

            {/* Barcodes */}
            <FormField
              control={form.control}
              name="codes"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Barcodes</FormLabel>
                  <FormControl>
                    <TagsInput
                      value={
                        Array.isArray(field.value)
                          ? field.value
                          : field.value
                          ? [String(field.value)]
                          : []
                      }
                      onChange={field.onChange}
                      placeholder="Enter barcode or PLU, press space or comma"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? "Saving…" : "Save"}
            </Button>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
