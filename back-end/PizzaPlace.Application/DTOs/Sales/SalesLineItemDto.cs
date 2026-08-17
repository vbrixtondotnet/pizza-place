namespace PizzaPlace.Application.DTOs.Sales;

public record SalesLineItemDto(
    int OrderDetailsId,
    int OrderId,
    DateOnly Date,
    TimeOnly Time,
    string PizzaId,
    string PizzaName,
    string Category,
    string Size,
    int Quantity,
    decimal UnitPrice,
    decimal LineRevenue);
