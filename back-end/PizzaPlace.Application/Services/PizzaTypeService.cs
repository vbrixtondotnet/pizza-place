using Microsoft.EntityFrameworkCore;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Application.DTOs;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Application.Services;

public class PizzaTypeService : IPizzaTypeService
{
    private readonly IApplicationDbContext _context;

    public PizzaTypeService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PizzaTypeDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.PizzaTypes
            .AsNoTracking()
            .OrderBy(pt => pt.PizzaTypeId)
            .Select(pt => new PizzaTypeDto(pt.PizzaTypeId, pt.Name, pt.Category, pt.Ingredients))
            .ToListAsync(cancellationToken);
    }

    public async Task<PizzaTypeDto?> GetByIdAsync(string pizzaTypeId, CancellationToken cancellationToken = default)
    {
        return await _context.PizzaTypes
            .AsNoTracking()
            .Where(pt => pt.PizzaTypeId == pizzaTypeId)
            .Select(pt => new PizzaTypeDto(pt.PizzaTypeId, pt.Name, pt.Category, pt.Ingredients))
            .FirstOrDefaultAsync(cancellationToken);
    }
}
