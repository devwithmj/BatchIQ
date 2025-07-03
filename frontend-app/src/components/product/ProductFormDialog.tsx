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
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { zodResolver } from "@hookform/resolvers/zod";
import * as z from "zod";

import { api } from "@/lib/api";
import { Pencil, Plus } from "lucide-react";
import { toast } from "sonner";

/* ------------------------------------------------------------------ */
/* 1️⃣  Zod schema + TS type                                           */
/* ------------------------------------------------------------------ */
const productSchema = z.object({
  nameEn: z.string().min(2),
  nameFa: z.string().min(2),
  brandEn: z.string().min(1),
  brandFa: z.string().min(1),
  sizeValue: z.coerce.number().positive(),
  unitType: z.enum(["g", "kg", "lb", "pcs"]),
  price: z.coerce.number().positive(),
});
type ProductFormValues = z.infer<typeof productSchema>;

/* ------------------------------------------------------------------ */
/* 2️⃣  Numeric enum map (match backend SizeUnit enum order)           */
/* ------------------------------------------------------------------ */
const unitMap = { g: 0, kg: 1, lb: 2, pcs: 3 } as const;
type UnitLabel = keyof typeof unitMap;

/* ------------------------------------------------------------------ */
/* 3️⃣  Component                                                      */
/* ------------------------------------------------------------------ */
type Props = {
  defaultValues?: Partial<ProductFormValues> & { id?: number };
  children?: React.ReactNode;
};

export default function ProductFormDialog({ defaultValues, children }: Props) {
  const isEdit = !!defaultValues?.id;
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();

  const form = useForm<ProductFormValues>({
    resolver: zodResolver(productSchema),
    defaultValues: {
      nameEn: "",
      nameFa: "",
      brandEn: "",
      brandFa: "",
      sizeValue: 0,
      unitType: "g",
      price: 0,
      ...defaultValues,
    },
  });

  /* ------------- mutation (create / update) ---------------------- */
  const mutation = useMutation({
    mutationFn: async (data: ProductFormValues) => {
      const payload = { ...data, unitType: unitMap[data.unitType] };

      if (isEdit) {
        await api.put(`/api/products/${defaultValues!.id}`, payload);
      } else {
        await api.post("/api/products", payload);
      }
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["inventory"] });
      toast.success("Saved ✅");
      setOpen(false);
    },
    onError: () =>
      toast.error( "Error saving product"),
  });

  /* --------------------------- UI -------------------------------- */
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
            onSubmit={form.handleSubmit((d) => mutation.mutate(d))}
          >
            {/* English & Persian names */}
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

            {/* English & Persian brands */}
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
                        type="number"
                        step=".01"
                        placeholder="400"
                        {...field}
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
                      onValueChange={field.onChange}
                      defaultValue={field.value}
                    >
                      <FormControl>
                        <SelectTrigger>
                          <SelectValue />
                        </SelectTrigger>
                      </FormControl>
                      <SelectContent>
                        {Object.keys(unitMap).map((u) => (
                          <SelectItem key={u} value={u}>
                            {u}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                    <FormMessage />
                  </FormItem>
                )}
              />
            </div>

            {/* Price */}
            <FormField
              control={form.control}
              name="price"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Price ($)</FormLabel>
                  <FormControl>
                    <Input
                      type="number"
                      step=".01"
                      placeholder="4.49"
                      {...field}
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />

            {/* Submit */}
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? "Saving…" : "Save"}
            </Button>
          </form>
        </Form>
      </DialogContent>
    </Dialog>
  );
}
