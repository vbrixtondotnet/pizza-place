using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PizzaTypesController : ControllerBase
{
    private readonly IPizzaTypeService _pizzaTypeService;

    public PizzaTypesController(IPizzaTypeService pizzaTypeService)
    {
        _pizzaTypeService = pizzaTypeService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var pizzaTypes = await _pizzaTypeService.GetAllAsync(cancellationToken);
        return Ok(pizzaTypes);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var pizzaType = await _pizzaTypeService.GetByIdAsync(id, cancellationToken);
        return pizzaType is null ? NotFound() : Ok(pizzaType);
    }
}
