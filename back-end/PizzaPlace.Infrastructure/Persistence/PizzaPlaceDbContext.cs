using Microsoft.EntityFrameworkCore;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Infrastructure.Persistence;

public class PizzaPlaceDbContext : DbContext, IApplicationDbContext
{
    public PizzaPlaceDbContext(DbContextOptions<PizzaPlaceDbContext> options)
        : base(options)
    {
    }

    public DbSet<PizzaType> PizzaTypes => Set<PizzaType>();
    public DbSet<Pizza> Pizzas => Set<Pizza>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetail> OrderDetails => Set<OrderDetail>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PizzaPlaceDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
