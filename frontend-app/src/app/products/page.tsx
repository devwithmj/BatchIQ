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

import { Product } from "@/lib/product-schema";
import BOMViewDialog from "@/components/product/BOMViewDialog";

export default function InventoryPage() {
  const [searchTerm, setSearchTerm] = useState("");
  
  const { data, isLoading } = useQuery<Product[]>({
    queryKey: ["inventory"],
    queryFn: async () => (await api.get("/api/products")).data,
  });

  // Filter products based on search term
  const filteredProducts = useMemo(() => {
    if (!data) return [];
    if (!searchTerm) return data;
    
    return data.filter((product) =>
      product.nameEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.nameFa.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.brandEn.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.brandFa.toLowerCase().includes(searchTerm.toLowerCase()) ||
      product.codes.some(code => code.toLowerCase().includes(searchTerm.toLowerCase()))
    );
  }, [data, searchTerm]);

  if (isLoading) return <p className="p-4">Loading…</p>;
  
  return (
    <AdminLayout>
      {/* Header with search and add button */}
      <div className="flex items-center justify-between mb-6">
        <div className="relative max-w-md flex-1">
          <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
          <Input
            placeholder="Search products by name, brand, or code..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-10"
          />
        </div>
        <ProductFormDialog /> {/* default is "Add" mode */}
      </div>

      {/* Products Table */}
      <Card>
        <CardHeader>
          <CardTitle>Products ({filteredProducts.length})</CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b border-border">
                  <th className="text-left p-3 font-medium text-muted-foreground">Product Name</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Brand</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Size</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Price</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Codes</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Manufactured</th>
                  <th className="text-right p-3 font-medium text-muted-foreground">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredProducts.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="text-center p-8 text-muted-foreground">
                      {searchTerm ? "No products found matching your search." : "No products available."}
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
                        <span className="font-mono">{product.sizeValue}</span>
                      </td>
                      <td className="p-3">
                        <span className="font-mono">${product.price}</span>
                      </td>
                      <td className="p-3">
                        <div className="flex flex-wrap gap-1">
                          {product.codes?.map((code, idx) => (
                            <span key={idx} className="bg-gray-100 px-2 py-1 rounded text-xs">
                              {code}
                            </span>
                          ))}
                        </div>
                      </td>
                      <td className="p-3">
                        {product.isManufactured && (
                          <span className="bg-blue-100 text-blue-800 px-2 py-1 rounded text-xs">
                            Manufactured
                          </span>
                        )}
                      </td>
                      <td className="p-3">
                        <div className="flex items-center justify-end gap-1">
                          <ProductFormDialog defaultValues={product}>
                            <Button variant="ghost" size="sm">
                              <Pencil className="h-4 w-4" />
                            </Button>
                          </ProductFormDialog>
                          {product.isManufactured && (
                            <BOMViewDialog 
                              productId={product.id} 
                              productName={product.nameEn}
                              productNameFa={product.nameFa}
                              isManufactured={product.isManufactured}
                              isProcessedProduct={product.isProcessedProduct}
                              productType={product.productType}
                            />
                          )}
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
