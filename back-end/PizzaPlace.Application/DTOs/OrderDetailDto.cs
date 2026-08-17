namespace PizzaPlace.Application.DTOs;

public record OrderDetailDto(
    int OrderDetailsId,
    int OrderId,
    string PizzaId,
    int Quantity);
