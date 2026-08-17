using Microsoft.AspNetCore.Mvc;
using PizzaPlace.Application.Interfaces;

namespace PizzaPlace.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderDetailsController : ControllerBase
{
    private readonly IOrderDetailService _orderDetailService;

    public OrderDetailsController(IOrderDetailService orderDetailService)
    {
        _orderDetailService = orderDetailService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var orderDetails = await _orderDetailService.GetAllAsync(cancellationToken);
        return Ok(orderDetails);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        var orderDetail = await _orderDetailService.GetByIdAsync(id, cancellationToken);
        return orderDetail is null ? NotFound() : Ok(orderDetail);
    }
}
