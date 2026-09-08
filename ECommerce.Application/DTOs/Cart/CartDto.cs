namespace ECommerce.Application.DTOs.Cart;

public class CartDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public IReadOnlyList<CartItemDto> Items { get; init; }
        = [];

    public decimal Subtotal =>
        Items.Sum(item => item.TotalPrice);
}