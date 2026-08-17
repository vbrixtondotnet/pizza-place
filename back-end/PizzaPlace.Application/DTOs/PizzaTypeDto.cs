namespace PizzaPlace.Application.DTOs;

public record PizzaTypeDto(
    string PizzaTypeId,
    string Name,
    string Category,
    string Ingredients);
