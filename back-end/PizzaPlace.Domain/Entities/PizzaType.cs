namespace PizzaPlace.Domain.Entities;

public class PizzaType
{
    public string PizzaTypeId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Ingredients { get; set; } = string.Empty;

    public ICollection<Pizza> Pizzas { get; set; } = new List<Pizza>();
}
