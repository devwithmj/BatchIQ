"use client";

import { useState, useEffect } from "react";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { reportApi } from "@/lib/report-api";
import { StockAvailabilityReport, ProductTransactionReport, TransactionSummary } from "@/lib/report-schema";
import { api } from "@/lib/api";
import { toast } from "sonner";
import { 
  BarChart3, 
  TrendingUp, 
  TrendingDown, 
  Package, 
  AlertTriangle,
  Search,
  RefreshCw
} from "lucide-react";
import AdminLayout from "@/components/layout/AdminLayout";

interface Product {
  id: number;
  nameEn: string;
  nameFa?: string;
}

export default function ReportsPage() {
  const [activeTab, setActiveTab] = useState<"stock" | "transactions">("stock");
  const [stockReports, setStockReports] = useState<StockAvailabilityReport[]>([]);
  const [selectedProduct, setSelectedProduct] = useState<number | null>(null);
  const [products, setProducts] = useState<Product[]>([]);
  const [productTransactions, setProductTransactions] = useState<ProductTransactionReport[]>([]);
  const [transactionSummary, setTransactionSummary] = useState<TransactionSummary | null>(null);
  const [isLoading, setIsLoading] = useState(false);
  const [filters, setFilters] = useState({
    lowStockOnly: false,
    needsReorderOnly: false,
    fromDate: "",
    toDate: "",
    transactionType: "",
  });

  useEffect(() => {
    fetchProducts();
    fetchStockReports();
  }, []); // eslint-disable-line react-hooks/exhaustive-deps

  const fetchProducts = async () => {
    try {
      const response = await api.get("/products");
      setProducts(response.data);
    } catch (error) {
      toast.error("Failed to fetch products");
      console.error("Error fetching products:", error);
    }
  };

  const fetchStockReports = async () => {
    try {
      setIsLoading(true);
      const reports = await reportApi.getStockAvailabilityReport({
        lowStockOnly: filters.lowStockOnly,
        needsReorderOnly: filters.needsReorderOnly,
      });
      setStockReports(reports);
    } catch (error) {
      toast.error("Failed to fetch stock reports");
      console.error("Error fetching stock reports:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const fetchProductTransactions = async (productId: number) => {
    try {
      setIsLoading(true);
      const [transactions, summary] = await Promise.all([
        reportApi.getProductTransactionReport(productId, {
          fromDate: filters.fromDate || undefined,
          toDate: filters.toDate || undefined,
          transactionType: filters.transactionType || undefined,
        }),
        reportApi.getProductTransactionSummary(productId, {
          fromDate: filters.fromDate || undefined,
          toDate: filters.toDate || undefined,
        }),
      ]);
      setProductTransactions(transactions);
      setTransactionSummary(summary);
    } catch (error) {
      toast.error("Failed to fetch product transactions");
      console.error("Error fetching product transactions:", error);
    } finally {
      setIsLoading(false);
    }
  };

  const handleProductSelect = (productId: string) => {
    const id = parseInt(productId);
    setSelectedProduct(id);
    if (activeTab === "transactions") {
      fetchProductTransactions(id);
    }
  };

  const formatDate = (dateString: string) => {
    return new Date(dateString).toLocaleDateString();
  };

  const formatDateTime = (dateString: string) => {
    return new Date(dateString).toLocaleString();
  };

  const getStockStatusBadge = (report: StockAvailabilityReport) => {
    if (report.needsReorder) {
      return <Badge variant="destructive" className="flex items-center gap-1">
        <AlertTriangle className="h-3 w-3" />
        Needs Reorder
      </Badge>;
    }
    if (report.isLowStock) {
      return <Badge variant="outline" className="flex items-center gap-1 text-yellow-600 border-yellow-600">
        <TrendingDown className="h-3 w-3" />
        Low Stock
      </Badge>;
    }
    return <Badge variant="default" className="flex items-center gap-1">
      <Package className="h-3 w-3" />
      Normal
    </Badge>;
  };

  return (
    <AdminLayout>
      <div className="space-y-6">
        <div className="flex items-center justify-between">
          <div>
            <h1 className="text-3xl font-bold tracking-tight">Reports</h1>
            <p className="text-muted-foreground">
              Analyze stock levels, product transactions, and business metrics
            </p>
          </div>
        </div>

        {/* Tab Navigation */}
        <div className="flex space-x-1 bg-muted p-1 rounded-lg w-fit">
          <Button
            variant={activeTab === "stock" ? "default" : "ghost"}
            onClick={() => setActiveTab("stock")}
            className="flex items-center gap-2"
          >
            <BarChart3 className="h-4 w-4" />
            Stock Reports
          </Button>
          <Button
            variant={activeTab === "transactions" ? "default" : "ghost"}
            onClick={() => setActiveTab("transactions")}
            className="flex items-center gap-2"
          >
            <TrendingUp className="h-4 w-4" />
            Transaction Reports
          </Button>
        </div>

        {/* Stock Reports Tab */}
        {activeTab === "stock" && (
          <div className="space-y-6">
            {/* Filters */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Search className="h-5 w-5" />
                  Stock Report Filters
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="flex flex-wrap gap-4">
                  <div className="flex items-center space-x-2">
                    <input
                      type="checkbox"
                      id="lowStock"
                      checked={filters.lowStockOnly}
                      onChange={(e) => setFilters({ ...filters, lowStockOnly: e.target.checked })}
                    />
                    <Label htmlFor="lowStock">Low Stock Only</Label>
                  </div>
                  <div className="flex items-center space-x-2">
                    <input
                      type="checkbox"
                      id="needsReorder"
                      checked={filters.needsReorderOnly}
                      onChange={(e) => setFilters({ ...filters, needsReorderOnly: e.target.checked })}
                    />
                    <Label htmlFor="needsReorder">Needs Reorder Only</Label>
                  </div>
                  <Button onClick={fetchStockReports} className="flex items-center gap-2">
                    <RefreshCw className="h-4 w-4" />
                    Apply Filters
                  </Button>
                </div>
              </CardContent>
            </Card>

            {/* Stock Reports Grid */}
            {isLoading ? (
              <div className="flex items-center justify-center h-32">
                <div className="text-center">
                  <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-gray-900 mx-auto"></div>
                  <p className="mt-2 text-sm text-gray-600">Loading stock reports...</p>
                </div>
              </div>
            ) : (
              <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-3">
                {stockReports.map((report) => (
                  <Card key={report.productId}>
                    <CardHeader>
                      <div className="flex items-center justify-between">
                        <CardTitle className="text-lg">
                          {report.productNameEn || report.productNameFa}
                        </CardTitle>
                        {getStockStatusBadge(report)}
                      </div>
                      <CardDescription>
                        {report.brandEn && `Brand: ${report.brandEn}`}
                      </CardDescription>
                    </CardHeader>
                    <CardContent className="space-y-4">
                      <div className="grid grid-cols-2 gap-4 text-sm">
                        <div>
                          <p className="font-medium">Current Stock</p>
                          <p className="text-2xl font-bold text-blue-600">
                            {report.currentStock} {report.baseUnit}
                          </p>
                        </div>
                        <div>
                          <p className="font-medium">Minimum Stock</p>
                          <p className="text-lg">
                            {report.minimumStock || "Not set"} {report.baseUnit}
                          </p>
                        </div>
                      </div>
                      
                      {report.locationBreakdown && report.locationBreakdown.length > 0 && (
                        <div>
                          <p className="font-medium text-sm mb-2">Location Breakdown</p>
                          <div className="space-y-1">
                            {report.locationBreakdown.map((location) => (
                              <div key={location.locationId} className="flex justify-between text-xs">
                                <span>{location.locationName}</span>
                                <span>{location.stock} {location.unit}</span>
                              </div>
                            ))}
                          </div>
                        </div>
                      )}
                      
                      <div className="text-xs text-muted-foreground">
                        Last Transaction: {formatDate(report.lastTransaction)}
                      </div>
                    </CardContent>
                  </Card>
                ))}
              </div>
            )}

            {stockReports.length === 0 && !isLoading && (
              <Card>
                <CardContent className="flex flex-col items-center justify-center py-12">
                  <Package className="h-12 w-12 text-muted-foreground mb-4" />
                  <h3 className="text-lg font-semibold mb-2">No Stock Reports Found</h3>
                  <p className="text-muted-foreground text-center">
                    No products match the current filter criteria.
                  </p>
                </CardContent>
              </Card>
            )}
          </div>
        )}

        {/* Transaction Reports Tab */}
        {activeTab === "transactions" && (
          <div className="space-y-6">
            {/* Product Selector */}
            <Card>
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <Search className="h-5 w-5" />
                  Transaction Report Filters
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-4">
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4">
                  <div>
                    <Label htmlFor="product">Product</Label>
                    <Select onValueChange={handleProductSelect}>
                      <SelectTrigger>
                        <SelectValue placeholder="Select a product" />
                      </SelectTrigger>
                      <SelectContent>
                        {products.map((product) => (
                          <SelectItem key={product.id} value={product.id.toString()}>
                            {product.nameEn || product.nameFa}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  </div>
                  
                  <div>
                    <Label htmlFor="fromDate">From Date</Label>
                    <Input
                      type="datetime-local"
                      value={filters.fromDate}
                      onChange={(e) => setFilters({ ...filters, fromDate: e.target.value })}
                    />
                  </div>
                  
                  <div>
                    <Label htmlFor="toDate">To Date</Label>
                    <Input
                      type="datetime-local"
                      value={filters.toDate}
                      onChange={(e) => setFilters({ ...filters, toDate: e.target.value })}
                    />
                  </div>
                  
                  <div>
                    <Label htmlFor="transactionType">Transaction Type</Label>
                    <Select
                      value={filters.transactionType || undefined}
                      onValueChange={(value) => setFilters({ ...filters, transactionType: value || "" })}
                    >
                      <SelectTrigger>
                        <SelectValue placeholder="All types" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="IN">In</SelectItem>
                        <SelectItem value="OUT">Out</SelectItem>
                        <SelectItem value="TRANSFER">Transfer</SelectItem>
                        <SelectItem value="ADJUSTMENT">Adjustment</SelectItem>
                        <SelectItem value="MANUFACTURING_IN">Manufacturing In</SelectItem>
                        <SelectItem value="MANUFACTURING_OUT">Manufacturing Out</SelectItem>
                      </SelectContent>
                    </Select>
                  </div>
                </div>
                
                {selectedProduct && (
                  <Button
                    onClick={() => fetchProductTransactions(selectedProduct)}
                    className="flex items-center gap-2"
                  >
                    <RefreshCw className="h-4 w-4" />
                    Apply Filters
                  </Button>
                )}
              </CardContent>
            </Card>

            {/* Transaction Summary */}
            {transactionSummary && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <BarChart3 className="h-5 w-5" />
                    Transaction Summary
                  </CardTitle>
                  <CardDescription>
                    Summary for {transactionSummary.productNameEn || transactionSummary.productNameFa}
                  </CardDescription>
                </CardHeader>
                <CardContent>
                  <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                    <div className="text-center">
                      <p className="text-2xl font-bold text-green-600">
                        {transactionSummary.totalIn}
                      </p>
                      <p className="text-sm text-muted-foreground">Total In</p>
                    </div>
                    <div className="text-center">
                      <p className="text-2xl font-bold text-red-600">
                        {transactionSummary.totalOut}
                      </p>
                      <p className="text-sm text-muted-foreground">Total Out</p>
                    </div>
                    <div className="text-center">
                      <p className="text-2xl font-bold text-blue-600">
                        {transactionSummary.netStock}
                      </p>
                      <p className="text-sm text-muted-foreground">Net Stock</p>
                    </div>
                    <div className="text-center">
                      <p className="text-2xl font-bold">
                        {transactionSummary.transactionCount}
                      </p>
                      <p className="text-sm text-muted-foreground">Transactions</p>
                    </div>
                  </div>
                </CardContent>
              </Card>
            )}

            {/* Transaction Details */}
            {productTransactions.length > 0 && (
              <Card>
                <CardHeader>
                  <CardTitle className="flex items-center gap-2">
                    <TrendingUp className="h-5 w-5" />
                    Transaction Details
                  </CardTitle>
                </CardHeader>
                <CardContent>
                  <div className="overflow-x-auto">
                    <table className="w-full border-collapse">
                      <thead>
                        <tr className="border-b">
                          <th className="text-left p-2">Date</th>
                          <th className="text-left p-2">Type</th>
                          <th className="text-left p-2">From</th>
                          <th className="text-left p-2">To</th>
                          <th className="text-right p-2">Quantity</th>
                          <th className="text-left p-2">Batch</th>
                        </tr>
                      </thead>
                      <tbody>
                        {productTransactions.map((transaction) => (
                          <tr key={transaction.transactionId} className="border-b hover:bg-muted/50">
                            <td className="p-2 text-sm">
                              {formatDateTime(transaction.timestamp)}
                            </td>
                            <td className="p-2">
                              <Badge variant="outline">{transaction.transactionType}</Badge>
                            </td>
                            <td className="p-2 text-sm">
                              {transaction.fromLocationName || "-"}
                            </td>
                            <td className="p-2 text-sm">
                              {transaction.toLocationName || "-"}
                            </td>
                            <td className="p-2 text-right font-medium">
                              {transaction.quantity} {transaction.unit}
                            </td>
                            <td className="p-2 text-sm">
                              {transaction.batchNumber || "-"}
                            </td>
                          </tr>
                        ))}
                      </tbody>
                    </table>
                  </div>
                </CardContent>
              </Card>
            )}

            {selectedProduct && productTransactions.length === 0 && !isLoading && (
              <Card>
                <CardContent className="flex flex-col items-center justify-center py-12">
                  <TrendingUp className="h-12 w-12 text-muted-foreground mb-4" />
                  <h3 className="text-lg font-semibold mb-2">No Transactions Found</h3>
                  <p className="text-muted-foreground text-center">
                    No transactions found for the selected product and filters.
                  </p>
                </CardContent>
              </Card>
            )}

            {!selectedProduct && (
              <Card>
                <CardContent className="flex flex-col items-center justify-center py-12">
                  <Package className="h-12 w-12 text-muted-foreground mb-4" />
                  <h3 className="text-lg font-semibold mb-2">Select a Product</h3>
                  <p className="text-muted-foreground text-center">
                    Choose a product from the dropdown to view its transaction history.
                  </p>
                </CardContent>
              </Card>
            )}
          </div>
        )}
      </div>
    </AdminLayout>
  );
}
