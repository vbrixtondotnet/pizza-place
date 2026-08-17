namespace PizzaPlace.Application.DTOs.Sales;

public record SalesDashboardDto(
    DateOnly FromDate,
    DateOnly ToDate,
    SalesKpiDto Kpis,
    SalesInsightsDto Insights,
    IReadOnlyList<DailySalesPointDto> DailyTrend,
    IReadOnlyList<CategorySalesDto> CategoryBreakdown,
    IReadOnlyList<TopPizzaDto> TopPizzas);
