using Microsoft.EntityFrameworkCore;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Application.DTOs;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Application.Services;

public class PizzaService : IPizzaService
{
    private readonly IApplicationDbContext _context;

    public PizzaService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PizzaDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .OrderBy(p => p.PizzaId)
            .Select(p => new PizzaDto(p.PizzaId, p.PizzaTypeId, p.Size, p.Price))
            .ToListAsync(cancellationToken);
    }

    public async Task<PizzaDto?> GetByIdAsync(string pizzaId, CancellationToken cancellationToken = default)
    {
        return await _context.Pizzas
            .AsNoTracking()
            .Where(p => p.PizzaId == pizzaId)
            .Select(p => new PizzaDto(p.PizzaId, p.PizzaTypeId, p.Size, p.Price))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
