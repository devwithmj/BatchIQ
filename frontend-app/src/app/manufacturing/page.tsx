"use client";
import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { api } from "@/lib/api";
import { useQuery } from "@tanstack/react-query";
import BOMViewDialog from "@/components/product/BOMViewDialog";
import { Search, Package, Factory } from "lucide-react";
import { useState, useMemo } from "react";

type Product = {
  id: number;
  nameEn: string;
  nameFa: string;
  brandEn: string;
  brandFa: string;
  productType: number;
  sizeValue: number;
  unitType: number;
  baseUnit: number;
  price: number;
  isManufactured: boolean;
  codes: string[];
};

export default function ManufacturingPage() {
  const [searchTerm, setSearchTerm] = useState("");
  
  const { data, isLoading } = useQuery<Product[]>({
    queryKey: ["manufactured-products"],
    queryFn: async () => (await api.get("/api/product-bom/manufactured-products")).data,
  });

  // Filter products based on search term
  const filteredProducts = useMemo(() => {
    if (!data) return [];
    if (!searchTerm) return data;
    
    return data.filter((product) =>
      product.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.nameFa.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.brandEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.brandFa.toLowerCase().includes(searchTerm.toLowerCase())
    );
  }, [data, searchTerm]);

  if (isLoading) return <p className="p-4">Loading…</p>;
  
  return (
    <AdminLayout>
      {/* Header with search */}
      <div className="flex items-center justify-between mb-6">
        <div className="flex items-center gap-4">
          <div className="flex items-center gap-2">
            <Factory className="h-6 w-6 text-blue-600" />
            <h1 className="text-2xl font-semibold">Manufacturing</h1>
          </div>
        </div>
        <div className="relative max-w-md flex-1 ml-4">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
          <Input
            placeholder="Search manufactured products..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
      </div>

      {/* Products Table */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <Package className="h-5 w-5" />
            Manufactured Products ({filteredProducts.length})
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b border-border">
                  <th className="text-left p-3 font-medium text-muted-foreground">Product Name</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Brand</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Type</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Size</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Price</th>
                  <th className="text-right p-3 font-medium text-muted-foreground">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredProducts.length === 0 ? (
                  <tr>
                    <td colSpan={6} className="text-center p-8 text-muted-foreground">
                      {searchTerm ? "No manufactured products found matching your search." : "No manufactured products available."}
                    </td>
                  </tr>
                ) : (
                  filteredProducts.map((product) => (
                    <tr key={product.id} className="border-b border-border hover:bg-muted/50 transition-colors">
                      <td className="p-3">
                        <div className="flex flex-col">
                          <span className="font-medium">{product.nameEn}</span>
                          <span className="text-sm text-muted-foreground">{product.nameFa}</span>
                        </div>
                      </td>
                      <td className="p-3">
                        <div className="flex flex-col">
                          <span className="font-medium">{product.brandEn}</span>
                          <span className="text-sm text-muted-foreground">{product.brandFa}</span>
                        </div>
                      </td>
                      <td className="p-3">
                        <Badge variant="secondary">
                          {getProductTypeLabel(product.productType)}
                        </Badge>
                      </td>
                      <td className="p-3">
                        <span className="font-mono">{product.sizeValue}</span>
                      </td>
                      <td className="p-3">
                        <span className="font-mono">${product.price}</span>
                      </td>
                      <td className="p-3">
                        <div className="flex items-center justify-end gap-1">
                          <BOMViewDialog 
                            productId={product.id} 
                            productName={product.nameEn}
                          >
                            <Button variant="outline" size="sm">
                              View BOM
                            </Button>
                          </BOMViewDialog>
                        </div>
                      </td>
                    </tr>
                  ))
                )}
              </tbody>
            </table>
          </div>
        </CardContent>
      </Card>
    </AdminLayout>
  );
}

function getProductTypeLabel(type: number): string {
  const types: Record<number, string> = {
    1: "Raw Material",
    2: "Ingredient", 
    3: "Finished Product",
    4: "Packaging",
  };
  return types[type] || "Unknown";
}
