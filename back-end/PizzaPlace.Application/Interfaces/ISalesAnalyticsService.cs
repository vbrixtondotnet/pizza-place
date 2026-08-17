using PizzaPlace.Application.DTOs.Sales;

namespace PizzaPlace.Application.Interfaces;

public interface ISalesAnalyticsService
{
    Task<SalesDashboardDto> GetDashboardAsync(
        DateOnly? fromDate,
        DateOnly? toDate,
        CancellationToken cancellationToken = default);

    Task<PagedResultDto<SalesLineItemDto>> SearchSalesAsync(
        SalesSearchQuery query,
        CancellationToken cancellationToken = default);

    Task<SalesFilterOptionsDto> GetFilterOptionsAsync(
        CancellationToken cancellationToken = default);
}
