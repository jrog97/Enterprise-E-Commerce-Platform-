using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Authorize(Roles = "Admin")]
public class AdminOrdersController : ControllerBase
{
    private readonly IOrderService _orderService;

    public AdminOrdersController(
        IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllOrders(
        CancellationToken cancellationToken)
    {
        var orders = await _orderService.GetAllOrdersAsync(
            cancellationToken);

        return Ok(orders);
    }

    [HttpPost("{orderId:guid}/confirm")]
    public async Task<IActionResult> Confirm(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.ConfirmOrderAsync(
            orderId,
            cancellationToken);

        return Ok(order);
    }

    [HttpPost("{orderId:guid}/processing")]
    public async Task<IActionResult> Processing(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.StartProcessingAsync(
            orderId,
            cancellationToken);

        return Ok(order);
    }

    [HttpPost("{orderId:guid}/ship")]
    public async Task<IActionResult> Ship(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.ShipOrderAsync(
            orderId,
            cancellationToken);

        return Ok(order);
    }

    [HttpPost("{orderId:guid}/deliver")]
    public async Task<IActionResult> Deliver(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.DeliverOrderAsync(
            orderId,
            cancellationToken);

        return Ok(order);
    }

    [HttpPost("{orderId:guid}/cancel")]
    public async Task<IActionResult> Cancel(
        Guid orderId,
        CancellationToken cancellationToken)
    {
        var order = await _orderService.CancelOrderAsync(
            orderId,
            cancellationToken);

        return Ok(order);
    }
}