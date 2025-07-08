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
import { TagsInput } from "../ui/taginput";

/* ------------------------------------------------------------------ */
/* 1️⃣  Zod schema & types – numbers are coerced from strings          */
/* ------------------------------------------------------------------ */
const schema = z.object({
  nameEn: z.string().min(2),
  nameFa: z.string().min(2),
  brandEn: z.string().min(1),
  brandFa: z.string().min(1),
  sizeValue: z.coerce.number().positive(),
  unitType: z.enum(["g", "kg", "lb", "pcs"]),
  price: z.coerce.number().positive(),
  codes: z.array(z.string().min(1)).optional(), // array of barcodes or PLUs
});
type FormValues = z.infer<typeof schema>;

/* enum mapping to backend numeric enum */
const unitMap = { g: 0, kg: 1, lb: 2, pcs: 3 } as const;
type UnitLabel = keyof typeof unitMap;

/* ------------------------------------------------------------------ */
/* 2️⃣  Dialog component                                               */
/* ------------------------------------------------------------------ */
type Props = {
  defaultValues?: Partial<FormValues> & { id?: number };
  children?: React.ReactNode;
};

export default function ProductFormDialog({ defaultValues, children }: Props) {
  const isEdit = !!defaultValues?.id;
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();
  /* ---------- always supply a value so inputs start controlled ----- */
  const form = useForm<FormValues>({
    resolver: zodResolver(schema),
    defaultValues: {
      nameEn: defaultValues?.nameEn ?? "",
      nameFa: defaultValues?.nameFa ?? "",
      brandEn: defaultValues?.brandEn ?? "",
      brandFa: defaultValues?.brandFa ?? "",
      sizeValue: defaultValues?.sizeValue?.toString() ?? "0",
      unitType: (defaultValues?.unitType as UnitLabel) ?? "g",
      price: defaultValues?.price?.toString() ?? "0",
      codes: defaultValues?.codes ?? [],
    } as any, // RHF accepts string→coerce
  });

  /* ---------------- mutation -------------------------------------- */
  const mutation = useMutation({
    mutationFn: async (data: FormValues) => {
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
    onError: () => toast("saved Failed ❌"),
  });

  /* utility to keep every Input controlled */
  const ctrl = (field: any, type = "text") => ({
    type,
    value: field.value ?? "",
    onChange: (e: any) => field.onChange(e.target.value),
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
            onSubmit={form.handleSubmit((d) => mutation.mutate(d))}
          >
            {/* Names */}
            <FormField
              control={form.control}
              name="nameEn"
              render={({ field }) => (
                <FormItem>
                  <FormLabel>Name (EN)</FormLabel>
                  <FormControl>
                    <Input placeholder="Fava Green Bean" {...ctrl(field)} />
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
                    <Input placeholder="لپه باقالا سبز" {...ctrl(field)} />
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
                      <Input placeholder="Pemina" {...ctrl(field)} />
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
                      <Input placeholder="پِمینا" {...ctrl(field)} />
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
                        placeholder="400"
                        {...ctrl(field, "number")}
                        step=".01"
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
                      placeholder="4.49"
                      {...ctrl(field, "number")}
                      step=".01"
                    />
                  </FormControl>
                  <FormMessage />
                </FormItem>
              )}
            />
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
