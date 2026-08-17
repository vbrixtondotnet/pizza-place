namespace PizzaPlace.Application.DTOs.Sales;

public record SalesKpiDto(
    decimal TotalRevenue,
    int OrderCount,
    int PizzasSold,
    decimal AverageOrderValue,
    decimal AveragePizzasPerOrder,
    decimal? RevenueChangePercent);
