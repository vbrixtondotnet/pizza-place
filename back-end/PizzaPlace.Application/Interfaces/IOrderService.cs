using PizzaPlace.Application.DTOs;

namespace PizzaPlace.Application.Interfaces;

public interface IOrderService
{
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default);
}
