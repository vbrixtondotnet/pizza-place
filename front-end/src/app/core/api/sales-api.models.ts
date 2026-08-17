export interface SalesKpi {
  totalRevenue: number;
  orderCount: number;
  pizzasSold: number;
  averageOrderValue: number;
  averagePizzasPerOrder: number;
  revenueChangePercent: number | null;
}

export interface SalesInsights {
  peakWeekday: string | null;
  peakHour: number | null;
  topCategory: string | null;
  topPizzaName: string | null;
}

export interface DailySalesPoint {
  date: string;
  revenue: number;
  orderCount: number;
  pizzasSold: number;
}

export interface CategorySales {
  category: string;
  revenue: number;
  pizzasSold: number;
  revenueSharePercent: number;
}

export interface TopPizza {
  pizzaName: string;
  category: string;
  quantitySold: number;
  revenue: number;
}

export interface SalesDashboard {
  fromDate: string;
  toDate: string;
  kpis: SalesKpi;
  insights: SalesInsights;
  dailyTrend: DailySalesPoint[];
  categoryBreakdown: CategorySales[];
  topPizzas: TopPizza[];
}

export interface SalesLineItem {
  orderDetailsId: number;
  orderId: number;
  date: string;
  time: string;
  pizzaId: string;
  pizzaName: string;
  category: string;
  size: string;
  quantity: number;
  unitPrice: number;
  lineRevenue: number;
}

export interface PagedResult<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export interface SalesFilterOptions {
  categories: string[];
  sizes: string[];
  minDate: string | null;
  maxDate: string | null;
}

export interface SalesSearchParams {
  fromDate?: string | null;
  toDate?: string | null;
  query?: string | null;
  category?: string | null;
  size?: string | null;
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
}

export interface DashboardQueryParams {
  fromDate?: string | null;
  toDate?: string | null;
  query?: string | null;
  category?: string | null;
  size?: string | null;
}
