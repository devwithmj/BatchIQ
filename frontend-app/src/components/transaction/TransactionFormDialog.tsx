"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useMutation, useQueryClient, useQuery } from "@tanstack/react-query";
import { toast } from "sonner";
import { Plus, Calendar } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import { api } from "@/lib/api";
import { 
  transactionSchema, 
  TransactionFormValues, 
  TransactionTypes 
} from "@/lib/transaction-schema";
import { Location } from "@/lib/location-schema";

type Product = {
  id: number;
  nameEn: string;
  nameFa: string;
  brandEn: string;
  brandFa: string;
};

type Props = {
  defaultValues?: Partial<TransactionFormValues> & { id?: number };
  children?: React.ReactNode;
};

export default function TransactionFormDialog({ defaultValues, children }: Props) {
  const isEdit = !!defaultValues?.id;
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();

  // Fetch products and locations for dropdowns
  const { data: products } = useQuery<Product[]>({
    queryKey: ["products"],
    queryFn: async () => (await api.get("/api/products")).data,
  });

  const { data: locations } = useQuery<Location[]>({
    queryKey: ["locations"],
    queryFn: async () => (await api.get("/api/locations")).data,
  });

  const form = useForm<TransactionFormValues>({
    resolver: zodResolver(transactionSchema),
    defaultValues: {
      productId: defaultValues?.productId ?? 0,
      fromLocationId: defaultValues?.fromLocationId ?? null,
      toLocationId: defaultValues?.toLocationId ?? null,
      quantity: defaultValues?.quantity ?? 0,
      transactionType: defaultValues?.transactionType ?? 1,
      expiryDate: defaultValues?.expiryDate ?? "",
    },
  });

  const mutation = useMutation({
    mutationFn: async (data: TransactionFormValues) => {
      const payload = {
        productId: data.productId,
        fromLocationId: data.fromLocationId || null,
        toLocationId: data.toLocationId || null,
        quantity: data.quantity,
        transactionType: data.transactionType,
        expiryDate: data.expiryDate || null,
      };

      if (isEdit) {
        return await api.put(`/api/transactions/${defaultValues?.id}`, payload);
      } else {
        return await api.post("/api/transactions", payload);
      }
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["transactions"] });
      setOpen(false);
      form.reset();
      toast(isEdit ? "Transaction Updated Successfully ✅" : "Transaction Created Successfully ✅");
    },
    onError: (error: Error) => {
      console.error("Transaction save error:", error);
      toast("Save Failed ❌");
    },
  });

  const onSubmit = (data: TransactionFormValues) => {
    mutation.mutate(data);
  };

  const watchedTransactionType = form.watch("transactionType");

  // Helper function to determine required fields based on transaction type
  const getLocationRequirements = (type: number) => {
    switch (type) {
      case 1: // Stock In
        return { fromRequired: false, toRequired: true };
      case 2: // Stock Out  
        return { fromRequired: true, toRequired: false };
      case 3: // Transfer
        return { fromRequired: true, toRequired: true };
      case 4: // Adjustment
        return { fromRequired: false, toRequired: false };
      default:
        return { fromRequired: false, toRequired: false };
    }
  };

  const locationReqs = getLocationRequirements(watchedTransactionType);

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {children || (
          <Button>
            <Plus className="h-4 w-4 mr-2" />
            Add Transaction
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>
            {isEdit ? "Edit Transaction" : "Add New Transaction"}
          </DialogTitle>
        </DialogHeader>
        <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-4">
          <div className="space-y-2">
            <Label htmlFor="productId">Product *</Label>
            <Select
              value={form.watch("productId")?.toString() || ""}
              onValueChange={(value) => form.setValue("productId", parseInt(value))}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select product" />
              </SelectTrigger>
              <SelectContent>
                {products?.map((product) => (
                  <SelectItem key={product.id} value={product.id.toString()}>
                    {product.nameEn} ({product.brandEn}) - {product.nameFa}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {form.formState.errors.productId && (
              <p className="text-sm text-red-600">
                {form.formState.errors.productId.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="transactionType">Transaction Type *</Label>
            <Select
              value={form.watch("transactionType")?.toString()}
              onValueChange={(value) => form.setValue("transactionType", parseInt(value))}
            >
              <SelectTrigger>
                <SelectValue placeholder="Select transaction type" />
              </SelectTrigger>
              <SelectContent>
                {Object.entries(TransactionTypes).map(([key, value]) => (
                  <SelectItem key={key} value={key}>
                    {value}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
            {form.formState.errors.transactionType && (
              <p className="text-sm text-red-600">
                {form.formState.errors.transactionType.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="quantity">Quantity *</Label>
            <Input
              id="quantity"
              type="number"
              step="0.01"
              {...form.register("quantity")}
              placeholder="Enter quantity"
            />
            {form.formState.errors.quantity && (
              <p className="text-sm text-red-600">
                {form.formState.errors.quantity.message}
              </p>
            )}
          </div>

          {(locationReqs.fromRequired || watchedTransactionType === 3) && (
            <div className="space-y-2">
              <Label htmlFor="fromLocationId">
                From Location {locationReqs.fromRequired ? "*" : ""}
              </Label>
              <Select
                value={form.watch("fromLocationId")?.toString() || "none"}
                onValueChange={(value) => 
                  form.setValue("fromLocationId", value === "none" ? null : parseInt(value))
                }
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select from location" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="none">No Location</SelectItem>
                  {locations?.map((location) => (
                    <SelectItem key={location.id} value={location.id.toString()}>
                      {location.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {form.formState.errors.fromLocationId && (
                <p className="text-sm text-red-600">
                  {form.formState.errors.fromLocationId.message}
                </p>
              )}
            </div>
          )}

          {(locationReqs.toRequired || watchedTransactionType === 3) && (
            <div className="space-y-2">
              <Label htmlFor="toLocationId">
                To Location {locationReqs.toRequired ? "*" : ""}
              </Label>
              <Select
                value={form.watch("toLocationId")?.toString() || "none"}
                onValueChange={(value) => 
                  form.setValue("toLocationId", value === "none" ? null : parseInt(value))
                }
              >
                <SelectTrigger>
                  <SelectValue placeholder="Select to location" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="none">No Location</SelectItem>
                  {locations?.map((location) => (
                    <SelectItem key={location.id} value={location.id.toString()}>
                      {location.name}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
              {form.formState.errors.toLocationId && (
                <p className="text-sm text-red-600">
                  {form.formState.errors.toLocationId.message}
                </p>
              )}
            </div>
          )}

          <div className="space-y-2">
            <Label htmlFor="expiryDate">Expiry Date (Optional)</Label>
            <div className="relative">
              <Calendar className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
              <Input
                id="expiryDate"
                type="datetime-local"
                {...form.register("expiryDate")}
                className="pl-10"
              />
            </div>
            {form.formState.errors.expiryDate && (
              <p className="text-sm text-red-600">
                {form.formState.errors.expiryDate.message}
              </p>
            )}
          </div>

          <div className="flex justify-end space-x-2 pt-4">
            <Button
              type="button"
              variant="outline"
              onClick={() => setOpen(false)}
            >
              Cancel
            </Button>
            <Button type="submit" disabled={mutation.isPending}>
              {mutation.isPending ? "Saving..." : isEdit ? "Update Transaction" : "Create Transaction"}
            </Button>
          </div>
        </form>
      </DialogContent>
    </Dialog>
  );
}
