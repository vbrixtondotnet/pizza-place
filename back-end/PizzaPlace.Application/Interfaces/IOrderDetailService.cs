using PizzaPlace.Application.DTOs;

namespace PizzaPlace.Application.Interfaces;

public interface IOrderDetailService
{
    Task<OrderDetailDto?> GetByIdAsync(int orderDetailsId, CancellationToken cancellationToken = default);
}
