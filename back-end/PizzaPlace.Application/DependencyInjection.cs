using Microsoft.Extensions.DependencyInjection;
using PizzaPlace.Application.Interfaces;
using PizzaPlace.Application.Services;

namespace PizzaPlace.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPizzaTypeService, PizzaTypeService>();
        services.AddScoped<IPizzaService, PizzaService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IOrderDetailService, OrderDetailService>();

        return services;
    }
}
