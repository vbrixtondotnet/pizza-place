namespace PizzaPlace.Application.DTOs.Sales;

public record PagedResultDto<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages);
