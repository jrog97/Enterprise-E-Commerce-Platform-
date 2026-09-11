namespace ECommerce.Application.Events;

public class OrderCreatedEvent
{
    public Guid EventId { get; init; } = Guid.NewGuid();

    public Guid OrderId { get; init; }

    public Guid UserId { get; init; }

    public decimal Total { get; init; }

    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}