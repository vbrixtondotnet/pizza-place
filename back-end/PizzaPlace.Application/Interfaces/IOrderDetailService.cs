using PizzaPlace.Application.DTOs;

namespace PizzaPlace.Application.Interfaces;

public interface IOrderDetailService
{
    Task<IReadOnlyList<OrderDetailDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDetailDto?> GetByIdAsync(int orderDetailsId, CancellationToken cancellationToken = default);
}
