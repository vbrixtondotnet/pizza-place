namespace PizzaPlace.Application.DTOs.Sales;

public record SalesFilterOptionsDto(
    IReadOnlyList<string> Categories,
    IReadOnlyList<string> Sizes,
    DateOnly? MinDate,
    DateOnly? MaxDate);
