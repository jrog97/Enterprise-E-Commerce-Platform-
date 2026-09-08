using ECommerce.Application.DTOs.Cart;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Customer")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(
        ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<ActionResult<CartDto>> GetCart(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var cart = await _cartService.GetCartAsync(
            userId,
            cancellationToken);

        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<ActionResult<CartDto>> AddItem(
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var cart = await _cartService.AddItemAsync(
            userId,
            request,
            cancellationToken);

        return Ok(cart);
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userId, out var parsedUserId))
        {
            throw new InvalidOperationException(
                "Authenticated user ID is invalid.");
        }

        return parsedUserId;
    }
}