using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace PizzaPlace.Infrastructure.Persistence.Seeding;

public static class DatabaseInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, string csvDirectory)
    {
        using var scope = services.CreateScope();
        var scopedServices = scope.ServiceProvider;
        var logger = scopedServices.GetRequiredService<ILoggerFactory>()
            .CreateLogger("DatabaseInitializer");

        try
        {
            var context = scopedServices.GetRequiredService<PizzaPlaceDbContext>();

            logger.LogInformation("Applying database migrations...");
            await context.Database.MigrateAsync();

            var seeder = scopedServices.GetRequiredService<CsvDataSeeder>();
            await seeder.SeedAsync(csvDirectory);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating or seeding the database.");
            throw;
        }
    }
}
