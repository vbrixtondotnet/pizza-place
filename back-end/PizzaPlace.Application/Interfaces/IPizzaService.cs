using PizzaPlace.Application.DTOs;

namespace PizzaPlace.Application.Interfaces;

public interface IPizzaService
{
    Task<IReadOnlyList<PizzaDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<PizzaDto?> GetByIdAsync(string pizzaId, CancellationToken cancellationToken = default);
}
