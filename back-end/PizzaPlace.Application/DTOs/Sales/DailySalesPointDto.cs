namespace PizzaPlace.Application.DTOs.Sales;

public record DailySalesPointDto(
    DateOnly Date,
    decimal Revenue,
    int OrderCount,
    int PizzasSold);
