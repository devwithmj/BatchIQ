"use client";

import { useState } from "react";
import { Eye } from "lucide-react";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Badge } from "@/components/ui/badge";
import { Separator } from "@/components/ui/separator";

import { Transaction, TransactionTypes } from "@/lib/transaction-schema";

type Props = {
  transaction: Transaction;
  children?: React.ReactNode;
};

export default function TransactionViewDialog({ transaction, children }: Props) {
  const [open, setOpen] = useState(false);

  const formatDate = (dateString?: string | null) => {
    if (!dateString) return "Not specified";
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
      second: "2-digit",
    });
  };

  const getTransactionTypeColor = (type: number) => {
    switch (type) {
      case 1: return "bg-green-100 text-green-800 border-green-200"; // Stock In
      case 2: return "bg-red-100 text-red-800 border-red-200";     // Stock Out
      case 3: return "bg-blue-100 text-blue-800 border-blue-200";   // Transfer
      case 4: return "bg-yellow-100 text-yellow-800 border-yellow-200"; // Adjustment
      default: return "bg-gray-100 text-gray-800 border-gray-200";
    }
  };

  const getTransactionDescription = (type: number) => {
    switch (type) {
      case 1: return "Inventory increase - items added to stock";
      case 2: return "Inventory decrease - items removed from stock";
      case 3: return "Location change - items moved between locations";
      case 4: return "Inventory correction - adjustment to stock levels";
      default: return "Unknown transaction type";
    }
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {children || (
          <Button variant="ghost" size="sm">
            <Eye className="h-4 w-4" />
          </Button>
        )}
      </DialogTrigger>
      <DialogContent className="sm:max-w-lg">
        <DialogHeader>
          <DialogTitle className="flex items-center space-x-2">
            <span>Transaction Details</span>
            <Badge className={getTransactionTypeColor(transaction.transactionType)}>
              {TransactionTypes[transaction.transactionType as keyof typeof TransactionTypes]}
            </Badge>
          </DialogTitle>
        </DialogHeader>
        
        <div className="space-y-6">
          {/* Transaction Type Description */}
          <div className="bg-muted/50 p-3 rounded-lg">
            <p className="text-sm text-muted-foreground">
              {getTransactionDescription(transaction.transactionType)}
            </p>
          </div>

          {/* Product Information */}
          <div className="space-y-3">
            <h3 className="font-semibold text-base">Product Information</h3>
            <div className="grid gap-2 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Product ID:</span>
                <span className="font-medium">#{transaction.productId}</span>
              </div>
              {transaction.productNameEn && (
                <>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Name (EN):</span>
                    <span className="font-medium">{transaction.productNameEn}</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-muted-foreground">Name (FA):</span>
                    <span className="font-medium">{transaction.productNameFa}</span>
                  </div>
                </>
              )}
            </div>
          </div>

          <Separator />

          {/* Transaction Details */}
          <div className="space-y-3">
            <h3 className="font-semibold text-base">Transaction Details</h3>
            <div className="grid gap-2 text-sm">
              <div className="flex justify-between">
                <span className="text-muted-foreground">Quantity:</span>
                <span className="font-medium text-lg">{transaction.quantity}</span>
              </div>
              
              {(transaction.fromLocationId || transaction.fromLocationName) && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">From Location:</span>
                  <span className="font-medium">
                    {transaction.fromLocationName || `Location #${transaction.fromLocationId}`}
                  </span>
                </div>
              )}
              
              {(transaction.toLocationId || transaction.toLocationName) && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">To Location:</span>
                  <span className="font-medium">
                    {transaction.toLocationName || `Location #${transaction.toLocationId}`}
                  </span>
                </div>
              )}
              
              <div className="flex justify-between">
                <span className="text-muted-foreground">Expiry Date:</span>
                <span className="font-medium">{formatDate(transaction.expiryDate)}</span>
              </div>
              
              {transaction.createdAt && (
                <div className="flex justify-between">
                  <span className="text-muted-foreground">Created At:</span>
                  <span className="font-medium">{formatDate(transaction.createdAt.toString())}</span>
                </div>
              )}
            </div>
          </div>

          {/* Transaction Flow Visualization */}
          {(transaction.fromLocationId || transaction.toLocationId) && (
            <>
              <Separator />
              <div className="space-y-3">
                <h3 className="font-semibold text-base">Transaction Flow</h3>
                <div className="flex items-center justify-center space-x-4">
                  <div className="text-center">
                    <div className="bg-muted p-3 rounded-lg min-w-24">
                      <div className="text-xs text-muted-foreground mb-1">FROM</div>
                      <div className="font-medium text-sm">
                        {transaction.fromLocationName || 
                         (transaction.fromLocationId ? `Location #${transaction.fromLocationId}` : "External")}
                      </div>
                    </div>
                  </div>
                  
                  <div className="flex-1 border-t-2 border-dashed border-muted-foreground relative">
                    <div className="absolute top-0 left-1/2 transform -translate-x-1/2 -translate-y-1/2 bg-background px-2">
                      <div className="text-xs text-muted-foreground">
                        {transaction.quantity} units
                      </div>
                    </div>
                    <div className="absolute top-0 right-0 transform -translate-y-1/2">
                      <div className="w-0 h-0 border-l-4 border-l-muted-foreground border-t-2 border-b-2 border-t-transparent border-b-transparent"></div>
                    </div>
                  </div>
                  
                  <div className="text-center">
                    <div className="bg-muted p-3 rounded-lg min-w-24">
                      <div className="text-xs text-muted-foreground mb-1">TO</div>
                      <div className="font-medium text-sm">
                        {transaction.toLocationName || 
                         (transaction.toLocationId ? `Location #${transaction.toLocationId}` : "External")}
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </>
          )}
        </div>

        <div className="flex justify-end pt-4">
          <Button variant="outline" onClick={() => setOpen(false)}>
            Close
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
