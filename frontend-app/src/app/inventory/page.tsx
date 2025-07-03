"use client";
import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { api } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";

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
      <div className="grid gap-4 p-4 md:grid-cols-2 xl:grid-cols-3">
        {data?.map((p) => (
          <Card key={p.id}>
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
