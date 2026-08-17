using PizzaPlace.Application.Common;
using PizzaPlace.Application.DTOs.Sales;
using PizzaPlace.Application.Services;

namespace PizzaPlace.Application.Tests;

public class SalesAnalyticsServiceTests
{
    [Fact]
    public async Task GetDashboardAsync_ComputesKpisAndPriorPeriodChange()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var dashboard = await service.GetDashboardAsync(new SalesDashboardQuery
        {
            FromDate = new DateOnly(2015, 1, 5),
            ToDate = new DateOnly(2015, 1, 11)
        });

        Assert.Equal(40m, dashboard.Kpis.TotalRevenue);
        Assert.Equal(2, dashboard.Kpis.OrderCount);
        Assert.Equal(3, dashboard.Kpis.PizzasSold);
        Assert.Equal(20m, dashboard.Kpis.AverageOrderValue);
        Assert.Equal(1.5m, dashboard.Kpis.AveragePizzasPerOrder);
        Assert.Equal(-33.33m, dashboard.Kpis.RevenueChangePercent);
        Assert.Equal("Classic", dashboard.Insights.TopCategory);
        Assert.Equal("Classic Margherita", dashboard.Insights.TopPizzaName);
        Assert.Equal(7, dashboard.DailyTrend.Count);
        Assert.Contains(dashboard.CategoryBreakdown, x => x.Category == "Classic" && x.Revenue == 20m);
        Assert.Contains(dashboard.TopPizzas, x => x.PizzaName == "BBQ Chicken");
    }

    [Fact]
    public async Task SearchSalesAsync_FiltersAndPaginates()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var page = await service.SearchSalesAsync(new SalesSearchQuery
        {
            FromDate = new DateOnly(2015, 1, 1),
            ToDate = new DateOnly(2015, 1, 31),
            Category = "Classic",
            Query = "margherita",
            Page = 1,
            PageSize = 10,
            SortBy = "lineRevenue",
            SortDirection = "desc"
        });

        Assert.Equal(2, page.TotalCount);
        Assert.Equal(2, page.Items.Count);
        Assert.All(page.Items, item => Assert.Equal("Classic", item.Category));
        Assert.True(page.Items[0].LineRevenue >= page.Items[1].LineRevenue);
    }

    [Fact]
    public async Task SearchSalesAsync_RejectsInvalidPaging()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            service.SearchSalesAsync(new SalesSearchQuery
            {
                Page = 0,
                PageSize = 25
            }));

        Assert.True(exception.Errors.ContainsKey("page"));
    }

    [Fact]
    public async Task SearchSalesAsync_RejectsInvalidSort()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            service.SearchSalesAsync(new SalesSearchQuery
            {
                SortBy = "unknown"
            }));

        Assert.True(exception.Errors.ContainsKey("sortBy"));
    }

    [Fact]
    public async Task GetDashboardAsync_RejectsInvertedDateRange()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var exception = await Assert.ThrowsAsync<ValidationException>(() =>
            service.GetDashboardAsync(new SalesDashboardQuery
            {
                FromDate = new DateOnly(2015, 2, 1),
                ToDate = new DateOnly(2015, 1, 1)
            }));

        Assert.True(exception.Errors.ContainsKey("fromDate"));
    }

    [Fact]
    public async Task GetDashboardAsync_AppliesCategoryAndSizeFiltersToInsights()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var dashboard = await service.GetDashboardAsync(new SalesDashboardQuery
        {
            FromDate = new DateOnly(2015, 1, 1),
            ToDate = new DateOnly(2015, 1, 31),
            Category = "Chicken",
            Size = "L"
        });

        Assert.Equal(20m, dashboard.Kpis.TotalRevenue);
        Assert.Equal(1, dashboard.Kpis.OrderCount);
        Assert.Equal(1, dashboard.Kpis.PizzasSold);
        Assert.Equal("Chicken", dashboard.Insights.TopCategory);
        Assert.Equal("BBQ Chicken", dashboard.Insights.TopPizzaName);
        Assert.Equal("Tuesday", dashboard.Insights.PeakWeekday);
        Assert.Equal(18, dashboard.Insights.PeakHour);
        Assert.Single(dashboard.CategoryBreakdown);
        Assert.Single(dashboard.TopPizzas);
        Assert.Equal(20m, dashboard.DailyTrend.Sum(x => x.Revenue));
    }

    [Fact]
    public async Task GetDashboardAsync_AppliesSearchTermToPriorPeriodComparison()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var dashboard = await service.GetDashboardAsync(new SalesDashboardQuery
        {
            FromDate = new DateOnly(2015, 1, 5),
            ToDate = new DateOnly(2015, 1, 11),
            Query = "chicken"
        });

        // Current window has one BBQ Chicken line at $20; the prior window has three at $60.
        Assert.Equal(20m, dashboard.Kpis.TotalRevenue);
        Assert.Equal(-66.67m, dashboard.Kpis.RevenueChangePercent);
    }

    [Fact]
    public async Task GetFilterOptionsAsync_ReturnsDistinctValuesAndBounds()
    {
        await using var context = await SalesTestFixture.CreateSeededContextAsync();
        var service = new SalesAnalyticsService(context);

        var options = await service.GetFilterOptionsAsync();

        Assert.Equal(["Chicken", "Classic"], options.Categories);
        Assert.Equal(["L", "M"], options.Sizes);
        Assert.Equal(new DateOnly(2014, 12, 29), options.MinDate);
        Assert.Equal(new DateOnly(2015, 1, 12), options.MaxDate);
    }
}
