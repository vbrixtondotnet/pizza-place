namespace PizzaPlace.Application.DTOs.Sales;

public record CategorySalesDto(
    string Category,
    decimal Revenue,
    int PizzasSold,
    decimal RevenueSharePercent);
