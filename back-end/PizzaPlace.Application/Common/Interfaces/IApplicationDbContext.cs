using Microsoft.EntityFrameworkCore;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<PizzaType> PizzaTypes { get; }
    DbSet<Pizza> Pizzas { get; }
    DbSet<Order> Orders { get; }
    DbSet<OrderDetail> OrderDetails { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
