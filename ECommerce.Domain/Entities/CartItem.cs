namespace ECommerce.Domain.Entities;

public class CartItem
{
    public Guid Id { get; private set; }

    public Guid CartId { get; private set; }

    public ShoppingCart Cart { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public int Quantity { get; private set; }

    private CartItem()
    {
    }

    public CartItem(
        Guid cartId,
        Guid productId,
        int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        CartId = cartId;
        ProductId = productId;
        Quantity = quantity;
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        Quantity = quantity;
    }
}