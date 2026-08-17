using PizzaPlace.Application.DTOs;

namespace PizzaPlace.Application.Interfaces;

public interface IPizzaTypeService
{
    Task<IReadOnlyList<PizzaTypeDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PizzaTypeDto?> GetByIdAsync(string pizzaTypeId, CancellationToken cancellationToken = default);
}
