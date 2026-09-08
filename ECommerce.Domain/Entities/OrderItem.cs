namespace ECommerce.Domain.Entities;

public class OrderItem
{
    public Guid Id { get; private set; }

    public Guid OrderId { get; private set; }

    public Order Order { get; private set; } = null!;

    public Guid ProductId { get; private set; }

    public Product Product { get; private set; } = null!;

    public string ProductName { get; private set; }

    public string SKU { get; private set; }

    public decimal UnitPrice { get; private set; }

    public int Quantity { get; private set; }

    public decimal TotalPrice =>
        UnitPrice * Quantity;

    private OrderItem()
    {
    }

    public OrderItem(
        Guid orderId,
        Guid productId,
        string productName,
        string sku,
        decimal unitPrice,
        int quantity)
    {
        if (unitPrice <= 0)
        {
            throw new ArgumentException(
                "Unit price must be greater than zero.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentException(
                "Quantity must be greater than zero.");
        }

        Id = Guid.NewGuid();
        OrderId = orderId;
        ProductId = productId;
        ProductName = productName;
        SKU = sku;
        UnitPrice = unitPrice;
        Quantity = quantity;
    }
}