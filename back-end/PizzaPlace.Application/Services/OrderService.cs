using Microsoft.EntityFrameworkCore;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Application.DTOs;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Application.Services;

public class OrderService : IOrderService
{
    private readonly IApplicationDbContext _context;

    public OrderService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .OrderBy(o => o.OrderId)
            .Select(o => new OrderDto(o.OrderId, o.Date, o.Time))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderDto?> GetByIdAsync(int orderId, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .AsNoTracking()
            .Where(o => o.OrderId == orderId)
            .Select(o => new OrderDto(o.OrderId, o.Date, o.Time))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
