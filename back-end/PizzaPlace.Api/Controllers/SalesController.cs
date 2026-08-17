using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Application.Common;
using PizzaPlace.Application.DTOs.Sales;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Api.Controllers;

[ApiController]
[Route("api/sales")]
public class SalesController : ControllerBase
{
    private readonly ISalesAnalyticsService _salesAnalyticsService;

    public SalesController(ISalesAnalyticsService salesAnalyticsService)
    {
        _salesAnalyticsService = salesAnalyticsService;
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(typeof(SalesDashboardDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetDashboard(
        [FromQuery] DateOnly? fromDate,
        [FromQuery] DateOnly? toDate,
        CancellationToken cancellationToken)
    {
        try
        {
            var dashboard = await _salesAnalyticsService.GetDashboardAsync(
                fromDate,
                toDate,
                cancellationToken);
            return Ok(dashboard);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(new ValidationProblemDetails(ex.Errors));
        }
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedResultDto<SalesLineItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search(
        [FromQuery] SalesSearchQuery query,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await _salesAnalyticsService.SearchSalesAsync(query, cancellationToken);
            return Ok(result);
        }
        catch (ValidationException ex)
        {
            return ValidationProblem(new ValidationProblemDetails(ex.Errors));
        }
    }

    [HttpGet("filters")]
    [ProducesResponseType(typeof(SalesFilterOptionsDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFilters(CancellationToken cancellationToken)
    {
        var filters = await _salesAnalyticsService.GetFilterOptionsAsync(cancellationToken);
        return Ok(filters);
    }
}
