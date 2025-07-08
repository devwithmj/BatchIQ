"use client";

import { useMemo } from "react";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { TrendingUp, TrendingDown, ArrowRightLeft, Settings } from "lucide-react";

import { Transaction } from "@/lib/transaction-schema";

type Props = {
  transactions: Transaction[];
};

export default function TransactionStats({ transactions }: Props) {
  const stats = useMemo(() => {
    if (!transactions.length) {
      return {
        totalTransactions: 0,
        stockIn: 0,
        stockOut: 0,
        transfers: 0,
        adjustments: 0,
        totalQuantityIn: 0,
        totalQuantityOut: 0,
        netQuantity: 0,
        recentTransactions: 0,
      };
    }

    const stockIn = transactions.filter(t => t.transactionType === 1);
    const stockOut = transactions.filter(t => t.transactionType === 2);
    const transfers = transactions.filter(t => t.transactionType === 3);
    const adjustments = transactions.filter(t => t.transactionType === 4);

    const totalQuantityIn = stockIn.reduce((sum, t) => sum + t.quantity, 0);
    const totalQuantityOut = stockOut.reduce((sum, t) => sum + t.quantity, 0);
    const netQuantity = totalQuantityIn - totalQuantityOut;

    // Recent transactions (last 24 hours)
    const yesterday = new Date();
    yesterday.setDate(yesterday.getDate() - 1);
    const recentTransactions = transactions.filter(t => 
      t.createdAt && new Date(t.createdAt.toString()) > yesterday
    ).length;

    return {
      totalTransactions: transactions.length,
      stockIn: stockIn.length,
      stockOut: stockOut.length,
      transfers: transfers.length,
      adjustments: adjustments.length,
      totalQuantityIn,
      totalQuantityOut,
      netQuantity,
      recentTransactions,
    };
  }, [transactions]);

  const StatCard = ({ 
    title, 
    value, 
    icon: Icon, 
    description, 
    color = "text-muted-foreground" 
  }: {
    title: string;
    value: string | number;
    icon: React.ElementType;
    description?: string;
    color?: string;
  }) => (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium">{title}</CardTitle>
        <Icon className={`h-4 w-4 ${color}`} />
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold">{value}</div>
        {description && (
          <p className="text-xs text-muted-foreground">{description}</p>
        )}
      </CardContent>
    </Card>
  );

  return (
    <div className="grid gap-4 md:grid-cols-2 lg:grid-cols-4">
      <StatCard
        title="Stock In"
        value={stats.stockIn}
        icon={TrendingUp}
        description={`+${stats.totalQuantityIn.toFixed(2)} units total`}
        color="text-green-600"
      />
      
      <StatCard
        title="Stock Out"
        value={stats.stockOut}
        icon={TrendingDown}
        description={`-${stats.totalQuantityOut.toFixed(2)} units total`}
        color="text-red-600"
      />
      
      <StatCard
        title="Transfers"
        value={stats.transfers}
        icon={ArrowRightLeft}
        description="Between locations"
        color="text-blue-600"
      />
      
      <StatCard
        title="Adjustments"
        value={stats.adjustments}
        icon={Settings}
        description="Inventory corrections"
        color="text-yellow-600"
      />
    </div>
  );
}
