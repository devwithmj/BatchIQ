"use client";
import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { api } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import ProductFormDialog from "@/components/product/ProductFormDialog";

type Product = {
  id: number;
  persianName: string;
  brand: string;
  size: number;
  sizeUnit: number; // 1=g,2=kg,...
  codes: { code: string }[];
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
                  nameFa: p.persianName,
                  brandFa: p.brand,
                  sizeValue: p.size,
                  unitType: SizeUnitText[p.sizeUnit] as "g" | "kg" | "lb" | "pcs",
                }}
              />
            </div>
            <CardHeader>
              <CardTitle className="flex flex-col">
                {p.persianName}
                <span className="text-sm text-muted-foreground">{p.brand}</span>
              </CardTitle>
            </CardHeader>
            <CardContent>
              <p className="text-sm">
                Size: {p.size} {SizeUnitText[p.sizeUnit]}
              </p>
              <p className="text-xs text-muted-foreground">
                Codes: {p.codes.map((c) => c.code).join(", ")}
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
