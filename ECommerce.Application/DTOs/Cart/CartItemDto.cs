namespace ECommerce.Application.DTOs.Cart;

public class CartItemDto
{
    public Guid Id { get; init; }

    public Guid ProductId { get; init; }

    public string ProductName { get; init; } = string.Empty;

    public string SKU { get; init; } = string.Empty;

    public decimal UnitPrice { get; init; }

    public int Quantity { get; init; }

    public decimal TotalPrice =>
        UnitPrice * Quantity;
}