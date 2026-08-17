namespace PizzaPlace.Application.DTOs.Sales;

public sealed class SalesSearchQuery
{
    public DateOnly? FromDate { get; init; }
    public DateOnly? ToDate { get; init; }
    public string? Query { get; init; }
    public string? Category { get; init; }
    public string? Size { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 25;
    public string SortBy { get; init; } = "date";
    public string SortDirection { get; init; } = "desc";
}
