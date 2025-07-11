import { api } from "./api";
import {
  StockAvailabilityReport,
  ProductTransactionReport,
  TransactionSummary,
  StockAvailabilityQuery,
  ProductTransactionQuery,
  TransactionSummaryQuery,
} from "./report-schema";

export const reportApi = {
  // Stock Availability Reports
  async getStockAvailabilityReport(params?: StockAvailabilityQuery): Promise<StockAvailabilityReport[]> {
    const queryParams = new URLSearchParams();
    if (params?.lowStockOnly) queryParams.append("lowStockOnly", "true");
    if (params?.needsReorderOnly) queryParams.append("needsReorderOnly", "true");
    
    const url = `/reports/stock-availability${queryParams.toString() ? `?${queryParams.toString()}` : ""}`;
    const response = await api.get(url);
    return response.data;
  },

  async getProductStockAvailability(productId: number): Promise<StockAvailabilityReport> {
    const response = await api.get(`/reports/stock-availability/${productId}`);
    return response.data;
  },

  // Product Transaction Reports
  async getProductTransactionReport(
    productId: number,
    params?: ProductTransactionQuery
  ): Promise<ProductTransactionReport[]> {
    const queryParams = new URLSearchParams();
    if (params?.fromDate) queryParams.append("fromDate", params.fromDate);
    if (params?.toDate) queryParams.append("toDate", params.toDate);
    if (params?.transactionType) queryParams.append("transactionType", params.transactionType);
    if (params?.locationId) queryParams.append("locationId", params.locationId.toString());
    
    const url = `/reports/product-transactions/${productId}${queryParams.toString() ? `?${queryParams.toString()}` : ""}`;
    const response = await api.get(url);
    return response.data;
  },

  async getProductTransactionSummary(
    productId: number,
    params?: TransactionSummaryQuery
  ): Promise<TransactionSummary> {
    const queryParams = new URLSearchParams();
    if (params?.fromDate) queryParams.append("fromDate", params.fromDate);
    if (params?.toDate) queryParams.append("toDate", params.toDate);
    
    const url = `/reports/product-transactions/summary/${productId}${queryParams.toString() ? `?${queryParams.toString()}` : ""}`;
    const response = await api.get(url);
    return response.data;
  },
};
