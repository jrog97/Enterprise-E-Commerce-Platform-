namespace ECommerce.Application.DTOs.Orders;

public class OrderDto
{
    public Guid Id { get; init; }

    public Guid UserId { get; init; }

    public string Status { get; init; } = string.Empty;

    public decimal Subtotal { get; init; }

    public decimal Tax { get; init; }

    public decimal Total { get; init; }

    public DateTime CreatedAt { get; init; }

    public IReadOnlyList<OrderItemDto> Items { get; init; }
        = [];
}