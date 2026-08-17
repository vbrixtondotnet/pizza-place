using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PizzaPlace.Domain.Entities;

namespace PizzaPlace.Infrastructure.Persistence.Seeding;

public class CsvDataSeeder
{
    private const int BatchSize = 1000;
    private readonly PizzaPlaceDbContext _context;
    private readonly ILogger<CsvDataSeeder> _logger;

    public CsvDataSeeder(PizzaPlaceDbContext context, ILogger<CsvDataSeeder> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task SeedAsync(string csvDirectory, CancellationToken cancellationToken = default)
    {
        if (await _context.PizzaTypes.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Database already contains data. Skipping CSV seed.");
            return;
        }

        _logger.LogInformation("Seeding database from CSV files in {CsvDirectory}", csvDirectory);

        await SeedPizzaTypesAsync(Path.Combine(csvDirectory, "pizza_types.csv"), cancellationToken);
        await SeedPizzasAsync(Path.Combine(csvDirectory, "pizzas.csv"), cancellationToken);
        await SeedOrdersAsync(Path.Combine(csvDirectory, "orders.csv"), cancellationToken);
        await SeedOrderDetailsAsync(Path.Combine(csvDirectory, "order_details.csv"), cancellationToken);

        _logger.LogInformation("CSV seeding completed.");
    }

    private async Task SeedPizzaTypesAsync(string filePath, CancellationToken cancellationToken)
    {
        var records = ReadCsv(filePath, csv => new PizzaType
        {
            PizzaTypeId = csv.GetField("pizza_type_id")!,
            Name = csv.GetField("name")!,
            Category = csv.GetField("category")!,
            Ingredients = csv.GetField("ingredients")!
        });

        await _context.PizzaTypes.AddRangeAsync(records, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Seeded {Count} pizza types", records.Count);
    }

    private async Task SeedPizzasAsync(string filePath, CancellationToken cancellationToken)
    {
        var records = ReadCsv(filePath, csv => new Pizza
        {
            PizzaId = csv.GetField("pizza_id")!,
            PizzaTypeId = csv.GetField("pizza_type_id")!,
            Size = csv.GetField("size")!,
            Price = decimal.Parse(csv.GetField("price")!, CultureInfo.InvariantCulture)
        });

        await _context.Pizzas.AddRangeAsync(records, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Seeded {Count} pizzas", records.Count);
    }

    private async Task SeedOrdersAsync(string filePath, CancellationToken cancellationToken)
    {
        var records = ReadCsv(filePath, csv => new Order
        {
            OrderId = int.Parse(csv.GetField("order_id")!, CultureInfo.InvariantCulture),
            Date = DateOnly.Parse(csv.GetField("date")!, CultureInfo.InvariantCulture),
            Time = TimeOnly.Parse(csv.GetField("time")!, CultureInfo.InvariantCulture)
        });

        await AddInBatchesAsync(records, cancellationToken);
        _logger.LogInformation("Seeded {Count} orders", records.Count);
    }

    private async Task SeedOrderDetailsAsync(string filePath, CancellationToken cancellationToken)
    {
        var records = ReadCsv(filePath, csv => new OrderDetail
        {
            OrderDetailsId = int.Parse(csv.GetField("order_details_id")!, CultureInfo.InvariantCulture),
            OrderId = int.Parse(csv.GetField("order_id")!, CultureInfo.InvariantCulture),
            PizzaId = csv.GetField("pizza_id")!,
            Quantity = int.Parse(csv.GetField("quantity")!, CultureInfo.InvariantCulture)
        });

        await AddInBatchesAsync(records, cancellationToken);
        _logger.LogInformation("Seeded {Count} order details", records.Count);
    }

    private async Task AddInBatchesAsync<TEntity>(List<TEntity> records, CancellationToken cancellationToken)
        where TEntity : class
    {
        for (var i = 0; i < records.Count; i += BatchSize)
        {
            var batch = records.Skip(i).Take(BatchSize);
            await _context.Set<TEntity>().AddRangeAsync(batch, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            _context.ChangeTracker.Clear();
        }
    }

    private static List<T> ReadCsv<T>(string filePath, Func<CsvReader, T> map)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"CSV seed file not found: {filePath}");
        }

        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null
        };

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, config);
        csv.Read();
        csv.ReadHeader();

        var results = new List<T>();
        while (csv.Read())
        {
            results.Add(map(csv));
        }

        return results;
    }
}
