"use client";

import { useState, useMemo } from "react";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { Search, Filter, Eye, Plus, Pencil, Trash2 } from "lucide-react";
import { toast } from "sonner";

import AdminLayout from "@/components/layout/AdminLayout";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import TransactionFormDialog from "@/components/transaction/TransactionFormDialog";
import TransactionViewDialog from "@/components/transaction/TransactionViewDialog";
import TransactionStats from "@/components/transaction/TransactionStats";

import { api } from "@/lib/api";
import { Transaction, TransactionTypes } from "@/lib/transaction-schema";

export default function TransactionsPage() {
  const [searchTerm, setSearchTerm] = useState("");
  const [filterType, setFilterType] = useState<string>("all");
  const qc = useQueryClient();

  const { data: transactions, isLoading } = useQuery<Transaction[]>({
    queryKey: ["transactions"],
    queryFn: async () => {
      const response = await api.get("/api/transactions");
      return response.data;
    },
  });

  const deleteMutation = useMutation({
    mutationFn: async (id: number) => {
      await api.delete(`/api/transactions/${id}`);
    },
    onSuccess: () => {
      qc.invalidateQueries({ queryKey: ["transactions"] });
      toast("Transaction Deleted ✅");
    },
    onError: () => {
      toast("Delete Failed ❌");
    },
  });

  const handleDelete = (id: number) => {
    if (confirm("Are you sure you want to delete this transaction? This action cannot be undone.")) {
      deleteMutation.mutate(id);
    }
  };

  // Filter transactions based on search term and type
  const filteredTransactions = useMemo(() => {
    if (!transactions) return [];
    
    let filtered = transactions;

    // Filter by search term
    if (searchTerm) {
      filtered = filtered.filter((transaction) =>
        transaction.productNameEn?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        transaction.productNameFa?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        transaction.fromLocationName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        transaction.toLocationName?.toLowerCase().includes(searchTerm.toLowerCase()) ||
        TransactionTypes[transaction.transactionType as keyof typeof TransactionTypes]
          ?.toLowerCase().includes(searchTerm.toLowerCase())
      );
    }

    // Filter by transaction type
    if (filterType !== "all") {
      filtered = filtered.filter(transaction => 
        transaction.transactionType.toString() === filterType
      );
    }

    return filtered;
  }, [transactions, searchTerm, filterType]);

  const formatDate = (dateString?: string | null) => {
    if (!dateString) return "N/A";
    return new Date(dateString).toLocaleDateString("en-US", {
      year: "numeric",
      month: "short",
      day: "numeric",
      hour: "2-digit",
      minute: "2-digit",
    });
  };

  const getTransactionTypeColor = (type: number) => {
    switch (type) {
      case 1: return "bg-green-100 text-green-800"; // Stock In
      case 2: return "bg-red-100 text-red-800";     // Stock Out
      case 3: return "bg-blue-100 text-blue-800";   // Transfer
      case 4: return "bg-yellow-100 text-yellow-800"; // Adjustment
      default: return "bg-gray-100 text-gray-800";
    }
  };

  if (isLoading) return <p className="p-4">Loading transactions...</p>;

  return (
    <AdminLayout>
      {/* Header with search, filter and add button */}
      <div className="flex items-center justify-between mb-6 gap-4">
        <div className="flex items-center space-x-4 flex-1">
          <div className="relative max-w-md flex-1">
            <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground h-4 w-4" />
            <Input
              placeholder="Search transactions by product, location, or type..."
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              className="pl-10"
            />
          </div>
          
          <div className="flex items-center space-x-2">
            <Filter className="h-4 w-4 text-muted-foreground" />
            <Select value={filterType} onValueChange={setFilterType}>
              <SelectTrigger className="w-48">
                <SelectValue placeholder="Filter by type" />
              </SelectTrigger>
              <SelectContent>
                <SelectItem value="all">All Types</SelectItem>
                {Object.entries(TransactionTypes).map(([key, value]) => (
                  <SelectItem key={key} value={key}>
                    {value}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>
          </div>
        </div>
        
        <TransactionFormDialog />
      </div>

      {/* Transactions Table */}
      <Card>
        <CardHeader>
          <CardTitle>
            Transactions ({filteredTransactions.length})
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="overflow-x-auto">
            <table className="w-full border-collapse">
              <thead>
                <tr className="border-b border-border">
                  <th className="text-left p-3 font-medium text-muted-foreground">Type</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Product</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Quantity</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">From</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">To</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Expiry Date</th>
                  <th className="text-left p-3 font-medium text-muted-foreground">Created</th>
                  <th className="text-right p-3 font-medium text-muted-foreground">Actions</th>
                </tr>
              </thead>
              <tbody>
                {filteredTransactions.length === 0 ? (
                  <tr>
                    <td colSpan={8} className="text-center p-8">
                      <div className="flex flex-col items-center space-y-3">
                        <div className="text-muted-foreground">
                          {searchTerm || filterType !== "all" 
                            ? "No transactions found matching your filters." 
                            : "No transactions available yet."
                          }
                        </div>
                        {(!searchTerm && filterType === "all") && (
                          <div className="text-sm text-muted-foreground max-w-md">
                            <p className="mb-2">
                              Get started by creating your first transaction below!
                            </p>
                          </div>
                        )}
                        <TransactionFormDialog>
                          <Button variant="outline">
                            <Plus className="h-4 w-4 mr-2" />
                            Create Your First Transaction
                          </Button>
                        </TransactionFormDialog>
                      </div>
                    </td>
                  </tr>
                ) : (
                  filteredTransactions.map((transaction) => (
                    <tr key={transaction.id} className="border-b border-border hover:bg-muted/50 transition-colors">
                      <td className="p-3">
                        <span className={`inline-block px-2 py-1 rounded text-xs font-medium ${getTransactionTypeColor(transaction.transactionType)}`}>
                          {TransactionTypes[transaction.transactionType as keyof typeof TransactionTypes]}
                        </span>
                      </td>
                      <td className="p-3">
                        <div className="flex flex-col">
                          <span className="font-medium">{transaction.productNameEn || `Product #${transaction.productId}`}</span>
                          <span className="text-sm text-muted-foreground">{transaction.productNameFa}</span>
                        </div>
                      </td>
                      <td className="p-3">
                        <span className="font-medium">{transaction.quantity}</span>
                      </td>
                      <td className="p-3">
                        <span className="text-sm">
                          {transaction.fromLocationName || "-"}
                        </span>
                      </td>
                      <td className="p-3">
                        <span className="text-sm">
                          {transaction.toLocationName || "-"}
                        </span>
                      </td>
                      <td className="p-3">
                        <span className="text-sm">
                          {formatDate(transaction.expiryDate)}
                        </span>
                      </td>
                      <td className="p-3">
                        <span className="text-sm">
                          {formatDate(transaction.createdAt?.toString())}
                        </span>
                      </td>
                      <td className="p-3 text-right">
                        <div className="flex items-center justify-end space-x-1">
                          <TransactionViewDialog transaction={transaction}>
                            <Button variant="ghost" size="sm">
                              <Eye className="h-4 w-4" />
                            </Button>
                          </TransactionViewDialog>
                          
                          <TransactionFormDialog
                            defaultValues={{
                              id: transaction.id,
                              productId: transaction.productId,
                              fromLocationId: transaction.fromLocationId,
                              toLocationId: transaction.toLocationId,
                              quantity: transaction.quantity,
                              transactionType: transaction.transactionType,
                              expiryDate: transaction.expiryDate,
                            }}
                          >
                            <Button variant="ghost" size="sm">
                              <Pencil className="h-4 w-4" />
                            </Button>
                          </TransactionFormDialog>
                          
                          <Button
                            variant="ghost"
                            size="sm"
                            onClick={() => handleDelete(transaction.id)}
                            disabled={deleteMutation.isPending}
                          >
                            <Trash2 className="h-4 w-4 text-red-500" />
                          </Button>
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

      {/* Summary Cards */}
      <div className="mt-6">
        <TransactionStats transactions={transactions || []} />
      </div>
    </AdminLayout>
  );
}
