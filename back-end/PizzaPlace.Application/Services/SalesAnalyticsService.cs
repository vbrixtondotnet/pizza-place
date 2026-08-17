using Microsoft.EntityFrameworkCore;
using PizzaPlace.Application.Common;
using PizzaPlace.Application.Common.Interfaces;
using PizzaPlace.Application.DTOs.Sales;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Application.Services;

public class SalesAnalyticsService : ISalesAnalyticsService
{
    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "date",
        "time",
        "orderId",
        "pizzaName",
        "category",
        "size",
        "quantity",
        "unitPrice",
        "lineRevenue"
    };

    private readonly IApplicationDbContext _context;

    public SalesAnalyticsService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SalesDashboardDto> GetDashboardAsync(
        SalesDashboardQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        var (resolvedFrom, resolvedTo) = await ResolveDateRangeAsync(
            query.FromDate,
            query.ToDate,
            cancellationToken);
        ValidateDateRange(resolvedFrom, resolvedTo);

        var periodLengthDays = resolvedTo.DayNumber - resolvedFrom.DayNumber + 1;
        var previousTo = resolvedFrom.AddDays(-1);
        var previousFrom = previousTo.AddDays(-(periodLengthDays - 1));

        var currentLines = await BuildSalesQuery(
                resolvedFrom,
                resolvedTo,
                query.Query,
                query.Category,
                query.Size)
            .ToListAsync(cancellationToken);

        var previousRevenue = await GetRevenueAsync(
            previousFrom,
            previousTo,
            query.Query,
            query.Category,
            query.Size,
            cancellationToken);

        var totalRevenue = currentLines.Sum(x => x.LineRevenue);
        var pizzasSold = currentLines.Sum(x => x.Quantity);
        var orderCount = currentLines.Select(x => x.OrderId).Distinct().Count();

        var averageOrderValue = orderCount == 0
            ? 0m
            : Math.Round(totalRevenue / orderCount, 2);

        var averagePizzasPerOrder = orderCount == 0
            ? 0m
            : Math.Round((decimal)pizzasSold / orderCount, 2);

        decimal? revenueChangePercent = null;
        if (previousRevenue > 0)
        {
            revenueChangePercent = Math.Round(
                (totalRevenue - previousRevenue) / previousRevenue * 100m,
                2);
        }
        else if (totalRevenue > 0 && previousRevenue == 0)
        {
            revenueChangePercent = 100m;
        }

        var dailyTrend = currentLines
            .GroupBy(x => x.Date)
            .ToDictionary(
                g => g.Key,
                g => new DailySalesPointDto(
                    g.Key,
                    g.Sum(x => x.LineRevenue),
                    g.Select(x => x.OrderId).Distinct().Count(),
                    g.Sum(x => x.Quantity)));

        // Fill missing days with zeros so charts remain continuous.
        var filledDailyTrend = new List<DailySalesPointDto>();
        for (var day = resolvedFrom; day <= resolvedTo; day = day.AddDays(1))
        {
            filledDailyTrend.Add(
                dailyTrend.TryGetValue(day, out var existing)
                    ? existing
                    : new DailySalesPointDto(day, 0m, 0, 0));
        }

        var categoryBreakdown = currentLines
            .GroupBy(x => x.Category)
            .Select(g =>
            {
                var revenue = g.Sum(x => x.LineRevenue);
                var share = totalRevenue == 0
                    ? 0m
                    : Math.Round(revenue / totalRevenue * 100m, 2);

                return new CategorySalesDto(
                    g.Key,
                    revenue,
                    g.Sum(x => x.Quantity),
                    share);
            })
            .OrderByDescending(x => x.Revenue)
            .ToList();

        var topPizzas = currentLines
            .GroupBy(x => new { x.PizzaName, x.Category })
            .Select(g => new TopPizzaDto(
                g.Key.PizzaName,
                g.Key.Category,
                g.Sum(x => x.Quantity),
                g.Sum(x => x.LineRevenue)))
            .OrderByDescending(x => x.Revenue)
            .ThenByDescending(x => x.QuantitySold)
            .Take(5)
            .ToList();

        var peakWeekday = currentLines
            .GroupBy(x => x.Date.DayOfWeek)
            .Select(g => new { Weekday = g.Key, Revenue = g.Sum(x => x.LineRevenue) })
            .OrderByDescending(x => x.Revenue)
            .Select(x => x.Weekday.ToString())
            .FirstOrDefault();

        var peakHour = currentLines
            .GroupBy(x => x.Time.Hour)
            .Select(g => new { Hour = g.Key, Revenue = g.Sum(x => x.LineRevenue) })
            .OrderByDescending(x => x.Revenue)
            .Select(x => (int?)x.Hour)
            .FirstOrDefault();

        var kpis = new SalesKpiDto(
            Math.Round(totalRevenue, 2),
            orderCount,
            pizzasSold,
            averageOrderValue,
            averagePizzasPerOrder,
            revenueChangePercent);

        var insights = new SalesInsightsDto(
            peakWeekday,
            peakHour,
            categoryBreakdown.FirstOrDefault()?.Category,
            topPizzas.FirstOrDefault()?.PizzaName);

        return new SalesDashboardDto(
            resolvedFrom,
            resolvedTo,
            kpis,
            insights,
            filledDailyTrend,
            categoryBreakdown,
            topPizzas);
    }

    public async Task<PagedResultDto<SalesLineItemDto>> SearchSalesAsync(
        SalesSearchQuery query,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(query);

        if (query.Page < 1)
        {
            throw new ValidationException("page", "Page must be greater than or equal to 1.");
        }

        if (query.PageSize is < 1 or > 100)
        {
            throw new ValidationException("pageSize", "Page size must be between 1 and 100.");
        }

        if (!AllowedSortFields.Contains(query.SortBy))
        {
            throw new ValidationException(
                "sortBy",
                $"Sort by must be one of: {string.Join(", ", AllowedSortFields.Order())}.");
        }

        if (!string.Equals(query.SortDirection, "asc", StringComparison.OrdinalIgnoreCase)
            && !string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationException("sortDirection", "Sort direction must be 'asc' or 'desc'.");
        }

        var (resolvedFrom, resolvedTo) = await ResolveDateRangeAsync(
            query.FromDate,
            query.ToDate,
            cancellationToken);
        ValidateDateRange(resolvedFrom, resolvedTo);

        var salesQuery = BuildSalesQuery(
            resolvedFrom,
            resolvedTo,
            query.Query,
            query.Category,
            query.Size);

        var totalCount = await salesQuery.CountAsync(cancellationToken);
        var isDescending = string.Equals(query.SortDirection, "desc", StringComparison.OrdinalIgnoreCase);

        salesQuery = query.SortBy.ToLowerInvariant() switch
        {
            "time" => isDescending
                ? salesQuery.OrderByDescending(x => x.Time).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.Time).ThenBy(x => x.OrderDetailsId),
            "orderid" => isDescending
                ? salesQuery.OrderByDescending(x => x.OrderId).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.OrderId).ThenBy(x => x.OrderDetailsId),
            "pizzaname" => isDescending
                ? salesQuery.OrderByDescending(x => x.PizzaName).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.PizzaName).ThenBy(x => x.OrderDetailsId),
            "category" => isDescending
                ? salesQuery.OrderByDescending(x => x.Category).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.Category).ThenBy(x => x.OrderDetailsId),
            "size" => isDescending
                ? salesQuery.OrderByDescending(x => x.Size).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.Size).ThenBy(x => x.OrderDetailsId),
            "quantity" => isDescending
                ? salesQuery.OrderByDescending(x => x.Quantity).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.Quantity).ThenBy(x => x.OrderDetailsId),
            "unitprice" => isDescending
                ? salesQuery.OrderByDescending(x => x.UnitPrice).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.UnitPrice).ThenBy(x => x.OrderDetailsId),
            "linerevenue" => isDescending
                ? salesQuery.OrderByDescending(x => x.LineRevenue).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.LineRevenue).ThenBy(x => x.OrderDetailsId),
            _ => isDescending
                ? salesQuery.OrderByDescending(x => x.Date).ThenByDescending(x => x.Time).ThenByDescending(x => x.OrderDetailsId)
                : salesQuery.OrderBy(x => x.Date).ThenBy(x => x.Time).ThenBy(x => x.OrderDetailsId)
        };

        var items = await salesQuery
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(x => new SalesLineItemDto(
                x.OrderDetailsId,
                x.OrderId,
                x.Date,
                x.Time,
                x.PizzaId,
                x.PizzaName,
                x.Category,
                x.Size,
                x.Quantity,
                x.UnitPrice,
                x.LineRevenue))
            .ToListAsync(cancellationToken);

        var totalPages = totalCount == 0
            ? 0
            : (int)Math.Ceiling(totalCount / (double)query.PageSize);

        return new PagedResultDto<SalesLineItemDto>(
            items,
            query.Page,
            query.PageSize,
            totalCount,
            totalPages);
    }

    public async Task<SalesFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await _context.PizzaTypes
            .AsNoTracking()
            .Select(x => x.Category)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var sizes = await _context.Pizzas
            .AsNoTracking()
            .Select(x => x.Size)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync(cancellationToken);

        var dateBounds = await _context.Orders
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                MinDate = (DateOnly?)g.Min(x => x.Date),
                MaxDate = (DateOnly?)g.Max(x => x.Date)
            })
            .FirstOrDefaultAsync(cancellationToken);

        return new SalesFilterOptionsDto(
            categories,
            sizes,
            dateBounds?.MinDate,
            dateBounds?.MaxDate);
    }

    private IQueryable<SalesLineRow> BuildSalesQuery(
        DateOnly fromDate,
        DateOnly toDate,
        string? searchTerm,
        string? category,
        string? size)
    {
        var salesQuery =
            from detail in _context.OrderDetails.AsNoTracking()
            join order in _context.Orders.AsNoTracking() on detail.OrderId equals order.OrderId
            join pizza in _context.Pizzas.AsNoTracking() on detail.PizzaId equals pizza.PizzaId
            join pizzaType in _context.PizzaTypes.AsNoTracking() on pizza.PizzaTypeId equals pizzaType.PizzaTypeId
            where order.Date >= fromDate && order.Date <= toDate
            select new SalesLineRow
            {
                OrderDetailsId = detail.OrderDetailsId,
                OrderId = detail.OrderId,
                Date = order.Date,
                Time = order.Time,
                PizzaId = detail.PizzaId,
                PizzaName = pizzaType.Name,
                Category = pizzaType.Category,
                Size = pizza.Size,
                Quantity = detail.Quantity,
                UnitPrice = pizza.Price,
                LineRevenue = detail.Quantity * pizza.Price
            };

        if (!string.IsNullOrWhiteSpace(category))
        {
            var trimmedCategory = category.Trim();
            salesQuery = salesQuery.Where(x => x.Category == trimmedCategory);
        }

        if (!string.IsNullOrWhiteSpace(size))
        {
            var trimmedSize = size.Trim();
            salesQuery = salesQuery.Where(x => x.Size == trimmedSize);
        }

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            var term = searchTerm.Trim().ToLower();
            salesQuery = salesQuery.Where(x =>
                x.PizzaName.ToLower().Contains(term)
                || x.Category.ToLower().Contains(term)
                || x.PizzaId.ToLower().Contains(term)
                || x.OrderId.ToString().Contains(term));
        }

        return salesQuery;
    }

    private async Task<(DateOnly From, DateOnly To)> ResolveDateRangeAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken)
    {
        if (fromDate.HasValue && toDate.HasValue)
        {
            return (fromDate.Value, toDate.Value);
        }

        var bounds = await _context.Orders
            .AsNoTracking()
            .GroupBy(_ => 1)
            .Select(g => new
            {
                MinDate = g.Min(x => x.Date),
                MaxDate = g.Max(x => x.Date)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (bounds is null)
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            return (fromDate ?? today, toDate ?? today);
        }

        return (fromDate ?? bounds.MinDate, toDate ?? bounds.MaxDate);
    }

    private static void ValidateDateRange(DateOnly fromDate, DateOnly toDate)
    {
        if (fromDate > toDate)
        {
            throw new ValidationException(
                "fromDate",
                "From date must be less than or equal to to date.");
        }
    }

    private async Task<decimal> GetRevenueAsync(
        DateOnly fromDate,
        DateOnly toDate,
        string? searchTerm,
        string? category,
        string? size,
        CancellationToken cancellationToken)
    {
        if (fromDate > toDate)
        {
            return 0m;
        }

        return await BuildSalesQuery(fromDate, toDate, searchTerm, category, size)
            .SumAsync(x => x.LineRevenue, cancellationToken);
    }

    private sealed class SalesLineRow
    {
        public int OrderDetailsId { get; init; }
        public int OrderId { get; init; }
        public DateOnly Date { get; init; }
        public TimeOnly Time { get; init; }
        public string PizzaId { get; init; } = string.Empty;
        public string PizzaName { get; init; } = string.Empty;
        public string Category { get; init; } = string.Empty;
        public string Size { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public decimal UnitPrice { get; init; }
        public decimal LineRevenue { get; init; }
    }
}
