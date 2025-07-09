"use client";

import { useState } from "react";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";

import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { api } from "@/lib/api";
import { BOMItem, sizeUnitLabels } from "@/lib/bom-schema";
import BOMFormDialog from "./BOMFormDialog";
import { Eye, Pencil, Trash2, Plus } from "lucide-react";
import { toast } from "sonner";

type Props = {
  productId: number;
  productName: string;
  children?: React.ReactNode;
};

export default function BOMViewDialog({ productId, productName, children }: Props) {
  const [open, setOpen] = useState(false);
  const qc = useQueryClient();

  // Fetch BOM items for this product
  const { data: bomItems, isLoading, error } = useQuery<BOMItem[]>({
    queryKey: ["product-bom", productId],
    queryFn: async () => {
      const response = await api.get(`/api/product-bom/product/${productId}`);
      // Ensure we return an array
      return Array.isArray(response.data) ? response.data : [];
    },
    enabled: open, // Only fetch when dialog is open
  });

  // Ensure bomItems is always an array
  const safeBomItems = Array.isArray(bomItems) ? bomItems : [];

  // Fetch material cost
  const { data: materialCost } = useQuery<{ totalCost: number }>({
    queryKey: ["material-cost", productId],
    queryFn: async () => (await api.get(`/api/product-bom/material-cost/${productId}`)).data,
    enabled: open,
  });

  const deleteMutation = useMutation({
    mutationFn: async (bomId: number) => {
      await api.delete(`/api/product-bom/${bomId}`);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["product-bom"] });
      qc.invalidateQueries({ queryKey: ["material-cost"] });
      toast.success("BOM item deleted ✅");
    },
    onError: (error) => {
      console.error("Error deleting BOM item:", error);
      toast.error("Failed to delete BOM item ❌");
    },
  });

  const handleDelete = (bomId: number) => {
    if (confirm("Are you sure you want to delete this BOM item?")) {
      deleteMutation.mutate(bomId);
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {children ?? (
          <Button variant="outline" size="sm">
            <Eye className="h-4 w-4" />
            <span className="sr-only">View BOM</span>
          </Button>
        )}
      </DialogTrigger>

      <DialogContent className="max-w-4xl">
        <DialogHeader>
          <DialogTitle className="text-lg font-medium">
            Bill of Materials: {productName}
          </DialogTitle>
        </DialogHeader>

        <div className="space-y-4">
          {/* Header with add button and cost summary */}
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-4">
              <BOMFormDialog parentProductId={productId}>
                <Button size="sm">
                  <Plus className="h-4 w-4 mr-2" />
                  Add Component
                </Button>
              </BOMFormDialog>
              {materialCost && typeof materialCost.totalCost === 'number' && !isNaN(materialCost.totalCost) && (
                <Badge variant="secondary" className="text-sm">
                  Total Material Cost: ${materialCost.totalCost.toFixed(2)}
                </Badge>
              )}
            </div>
          </div>

          {/* BOM Items Table */}
          {isLoading ? (
            <div className="text-center py-8 text-muted-foreground">Loading...</div>
          ) : error ? (
            <div className="text-center py-8 text-red-500">
              Error loading BOM items. Please try again.
            </div>
          ) : safeBomItems.length === 0 ? (
            <div className="text-center py-8 text-muted-foreground">
              No components added yet. Click "Add Component" to get started.
            </div>
          ) : (
            <div className="overflow-x-auto">
              <table className="w-full border-collapse">
                <thead>
                  <tr className="border-b border-border">
                    <th className="text-left p-3 font-medium text-muted-foreground">Seq</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Component</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Brand</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Quantity</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Unit</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Cost/Unit</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Total Cost</th>
                    <th className="text-left p-3 font-medium text-muted-foreground">Critical</th>
                    <th className="text-right p-3 font-medium text-muted-foreground">Actions</th>
                  </tr>
                </thead>
                <tbody>
                  {safeBomItems
                    .sort((a, b) => (a.sequence || 999) - (b.sequence || 999))
                    .map((item) => (
                      <tr key={item.id} className="border-b border-border hover:bg-muted/50 transition-colors">
                        <td className="p-3 text-center">
                          {item.sequence || "-"}
                        </td>
                        <td className="p-3">
                          <div className="flex flex-col">
                            <span className="font-medium">{item.componentProduct.nameEn}</span>
                            <span className="text-sm text-muted-foreground">{item.componentProduct.nameFa}</span>
                          </div>
                        </td>
                        <td className="p-3">
                          <div className="flex flex-col">
                            <span className="font-medium">{item.componentProduct.brandEn}</span>
                            <span className="text-sm text-muted-foreground">{item.componentProduct.brandFa}</span>
                          </div>
                        </td>
                        <td className="p-3 font-mono">
                          {item.quantityRequired}
                        </td>
                        <td className="p-3">
                          {sizeUnitLabels[item.unit as keyof typeof sizeUnitLabels] || item.unit}
                        </td>
                        <td className="p-3 font-mono">
                          {item.costPerUnit ? `$${item.costPerUnit.toFixed(2)}` : "-"}
                        </td>
                        <td className="p-3 font-mono">
                          {item.costPerUnit 
                            ? `$${(item.quantityRequired * item.costPerUnit).toFixed(2)}`
                            : "-"
                          }
                        </td>
                        <td className="p-3">
                          {item.isCritical && (
                            <Badge variant="destructive" className="text-xs">Critical</Badge>
                          )}
                        </td>
                        <td className="p-3">
                          <div className="flex items-center justify-end gap-1">
                            <BOMFormDialog 
                              parentProductId={productId} 
                              defaultValues={item}
                            >
                              <Button variant="ghost" size="sm">
                                <Pencil className="h-4 w-4" />
                              </Button>
                            </BOMFormDialog>
                            <Button 
                              variant="ghost" 
                              size="sm"
                              onClick={() => handleDelete(item.id)}
                              disabled={deleteMutation.isPending}
                            >
                              <Trash2 className="h-4 w-4" />
                            </Button>
                          </div>
                        </td>
                      </tr>
                    ))}
                </tbody>
              </table>
            </div>
          )}

          {/* Notes section if any item has notes */}
          {safeBomItems.some(item => item.notes) && (
            <div className="space-y-2">
              <h4 className="font-medium">Notes:</h4>
              <div className="space-y-1">
                {safeBomItems
                  .filter(item => item.notes)
                  .map((item) => (
                    <div key={item.id} className="text-sm text-muted-foreground">
                      <span className="font-medium">{item.componentProduct.nameEn}:</span> {item.notes}
                    </div>
                  ))}
              </div>
            </div>
          )}
        </div>
      </DialogContent>
    </Dialog>
  );
}
