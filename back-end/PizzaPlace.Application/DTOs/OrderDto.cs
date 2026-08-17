namespace PizzaPlace.Application.DTOs;

public record OrderDto(
    int OrderId,
    DateOnly Date,
    TimeOnly Time);
