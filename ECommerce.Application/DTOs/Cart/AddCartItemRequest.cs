namespace ECommerce.Application.DTOs.Cart;

public class AddCartItemRequest
{
    public Guid ProductId { get; init; }

    public int Quantity { get; init; }
}