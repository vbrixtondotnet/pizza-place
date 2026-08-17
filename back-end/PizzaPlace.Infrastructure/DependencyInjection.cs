using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Infrastructure.Persistence;
using PizzaPlace.Infrastructure.Persistence.Seeding;

namespace PizzaPlace.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");

        services.AddDbContext<PizzaPlaceDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IApplicationDbContext>(provider =>
            provider.GetRequiredService<PizzaPlaceDbContext>());

        services.AddScoped<CsvDataSeeder>();

        return services;
    }
}
