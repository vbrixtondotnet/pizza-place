namespace PizzaPlace.Application.DTOs.Sales;

public record SalesInsightsDto(
    string? PeakWeekday,
    int? PeakHour,
    string? TopCategory,
    string? TopPizzaName);
