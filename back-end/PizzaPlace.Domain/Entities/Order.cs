namespace PizzaPlace.Domain.Entities;

public class Order
{
    public int OrderId { get; set; }
    public DateOnly Date { get; set; }
    public TimeOnly Time { get; set; }

    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
