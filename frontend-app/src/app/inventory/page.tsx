"use client";
import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { api } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import ProductFormDialog from "@/components/product/ProductFormDialog";
import { Search, Pencil } from "lucide-react";
import { useState, useMemo } from "react";

type Product = {
  id: number;
  nameEn: string;
  nameFa: string;
  brandEn: string;
  brandFa: string;
  sizeValue: number;
  unitType: number; // 1=g,2=kg,...
  price: number;
  codes: string[]; // array of barcodes or PLUs
};

export default function InventoryPage() {
  const { data, isLoading } = useQuery<Product[]>({
    queryKey: ["inventory"],
    queryFn: async () => (await api.get("/api/products")).data,
  });
  if (isLoading) return <p className="p-4">Loading…</p>;
  return (
    <AdminLayout>
      <div className="flex justify-end mb-4">
        <ProductFormDialog /> {/* default is "Add" mode */}
      </div>
      <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
        {data?.map((p) => (
          <Card key={p.id} className="relative group">
            {/* edit button floating top-right */}
            <div className="absolute top-2 right-2 opacity-0 group-hover:opacity-100 transition">
              <ProductFormDialog
                defaultValues={{
                  id: p.id,
                  nameEn: p.nameEn,
                  brandEn: p.brandEn,
                  sizeValue: p.sizeValue,
                  price: p.price,
                  nameFa: p.nameFa,
                  brandFa: p.brandFa,
                  codes: p.codes || [],
                }}
              />
            </div>
            <CardHeader>
              <CardTitle className="flex flex-col">
                {p.nameEn} ({p.nameFa})
                <span className="text-sm text-muted-foreground">
                  {p.brandEn} ({p.brandFa})
                </span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm">
                Size: {p.sizeValue} {SizeUnitText[p.unitType]}
              </p>
              <p className="text-xs text-muted-foreground">
                Codes: {p.codes.join(", ")}
              </p>
            </CardContent>
          </Card>
        ))}
      </div>
    </AdminLayout>
  );
}

const SizeUnitText: Record<number, string> = {
  1: "g",
  2: "kg",
  3: "lb",
  4: "pcs",
};
