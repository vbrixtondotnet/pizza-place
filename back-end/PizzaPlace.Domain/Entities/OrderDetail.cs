namespace PizzaPlace.Domain.Entities;

public class OrderDetail
{
    public int OrderDetailsId { get; set; }
    public int OrderId { get; set; }
    public string PizzaId { get; set; } = string.Empty;
    public int Quantity { get; set; }

    public Order Order { get; set; } = null!;
    public Pizza Pizza { get; set; } = null!;
}
