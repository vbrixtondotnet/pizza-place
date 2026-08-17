using Microsoft.EntityFrameworkCore;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Application.DTOs;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Application.Services;

public class OrderDetailService : IOrderDetailService
{
    private readonly IApplicationDbContext _context;

    public OrderDetailService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<OrderDetailDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.OrderDetails
            .AsNoTracking()
            .OrderBy(od => od.OrderDetailsId)
            .Select(od => new OrderDetailDto(od.OrderDetailsId, od.OrderId, od.PizzaId, od.Quantity))
            .ToListAsync(cancellationToken);
    }

    public async Task<OrderDetailDto?> GetByIdAsync(int orderDetailsId, CancellationToken cancellationToken = default)
    {
        return await _context.OrderDetails
            .AsNoTracking()
            .Where(od => od.OrderDetailsId == orderDetailsId)
            .Select(od => new OrderDetailDto(od.OrderDetailsId, od.OrderId, od.PizzaId, od.Quantity))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
