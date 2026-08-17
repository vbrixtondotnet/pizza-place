using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzasController : ControllerBase
{
    private readonly IPizzaService _pizzaService;

    public PizzasController(IPizzaService pizzaService)
    {
        _pizzaService = pizzaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var pizzas = await _pizzaService.GetAllAsync(cancellationToken);
        return Ok(pizzas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var pizza = await _pizzaService.GetByIdAsync(id, cancellationToken);
        return pizza is null ? NotFound() : Ok(pizza);
    }
}
