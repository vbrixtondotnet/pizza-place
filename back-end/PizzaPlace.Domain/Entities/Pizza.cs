namespace PizzaPlace.Domain.Entities;

public class Pizza
{
    public string PizzaId { get; set; } = string.Empty;
    public string PizzaTypeId { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Price { get; set; }

    public PizzaType PizzaType { get; set; } = null!;
    public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
