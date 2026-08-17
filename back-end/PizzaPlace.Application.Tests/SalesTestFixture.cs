using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using PizzaPlace.Domain.Entities;
using PizzaPlace.Infrastructure.Persistence;

namespace PizzaPlace.Application.Tests;

internal static class SalesTestFixture
{
    public static async Task<PizzaPlaceDbContext> CreateSeededContextAsync()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<PizzaPlaceDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new PizzaPlaceDbContext(options);
        await context.Database.EnsureCreatedAsync();

        context.PizzaTypes.AddRange(
            new PizzaType
            {
                PizzaTypeId = "classic_m",
                Name = "Classic Margherita",
                Category = "Classic",
                Ingredients = "Cheese, Tomato"
            },
            new PizzaType
            {
                PizzaTypeId = "chicken_b",
                Name = "BBQ Chicken",
                Category = "Chicken",
                Ingredients = "Chicken, BBQ"
            });

        context.Pizzas.AddRange(
            new Pizza
            {
                PizzaId = "classic_m_m",
                PizzaTypeId = "classic_m",
                Size = "M",
                Price = 10m
            },
            new Pizza
            {
                PizzaId = "chicken_b_l",
                PizzaTypeId = "chicken_b",
                Size = "L",
                Price = 20m
            });

        context.Orders.AddRange(
            new Order
            {
                OrderId = 1,
                Date = new DateOnly(2015, 1, 5),
                Time = new TimeOnly(12, 0)
            },
            new Order
            {
                OrderId = 2,
                Date = new DateOnly(2015, 1, 6),
                Time = new TimeOnly(18, 30)
            },
            new Order
            {
                OrderId = 3,
                Date = new DateOnly(2015, 1, 12),
                Time = new TimeOnly(18, 0)
            },
            new Order
            {
                OrderId = 4,
                Date = new DateOnly(2014, 12, 29),
                Time = new TimeOnly(11, 0)
            });

        context.OrderDetails.AddRange(
            new OrderDetail
            {
                OrderDetailsId = 1,
                OrderId = 1,
                PizzaId = "classic_m_m",
                Quantity = 2
            },
            new OrderDetail
            {
                OrderDetailsId = 2,
                OrderId = 2,
                PizzaId = "chicken_b_l",
                Quantity = 1
            },
            new OrderDetail
            {
                OrderDetailsId = 3,
                OrderId = 3,
                PizzaId = "classic_m_m",
                Quantity = 1
            },
            new OrderDetail
            {
                OrderDetailsId = 4,
                OrderId = 4,
                PizzaId = "chicken_b_l",
                Quantity = 3
            });

        await context.SaveChangesAsync();
        return context;
    }
}
