namespace PizzaPlace.Application.DTOs.Sales;

public record TopPizzaDto(
    string PizzaName,
    string Category,
    int QuantitySold,
    decimal Revenue);
