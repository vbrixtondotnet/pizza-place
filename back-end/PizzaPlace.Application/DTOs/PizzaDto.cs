namespace PizzaPlace.Application.DTOs;

public record PizzaDto(
    string PizzaId,
    string PizzaTypeId,
    string Size,
    decimal Price);
