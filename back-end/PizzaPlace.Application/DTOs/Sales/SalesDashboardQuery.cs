namespace PizzaPlace.Application.DTOs.Sales;

public sealed class SalesDashboardQuery
{
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public string? Query { get; init; }
    public string? Category { get; init; }
    public string? Size { get; init; }
}
