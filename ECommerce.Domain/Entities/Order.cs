using ECommerce.Domain.Enums;

namespace ECommerce.Domain.Entities;

public class Order
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public ApplicationUser User { get; private set; } = null!;

    public OrderStatus Status { get; private set; }

    public decimal Subtotal { get; private set; }

    public decimal Tax { get; private set; }

    public decimal Total { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime UpdatedAt { get; private set; }

    public ICollection<OrderItem> Items { get; private set; }
        = new List<OrderItem>();

    private Order()
    {
    }

    public Order(
        Guid userId,
        decimal subtotal,
        decimal tax)
    {
        if (subtotal < 0)
        {
            throw new ArgumentException(
                "Subtotal cannot be negative.");
        }

        if (tax < 0)
        {
            throw new ArgumentException(
                "Tax cannot be negative.");
        }

        Id = Guid.NewGuid();
        UserId = userId;
        Status = OrderStatus.Pending;
        Subtotal = subtotal;
        Tax = tax;
        Total = subtotal + tax;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Confirm()
    {
        if (Status != OrderStatus.Pending)
        {
            throw new InvalidOperationException(
                "Only pending orders can be confirmed.");
        }

        Status = OrderStatus.Confirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == OrderStatus.Shipped ||
            Status == OrderStatus.Delivered)
        {
            throw new InvalidOperationException(
                "This order can no longer be cancelled.");
        }

        Status = OrderStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }
}