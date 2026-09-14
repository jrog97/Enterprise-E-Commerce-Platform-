namespace ECommerce.Application.Events;

public class DeadLetterEvent
{
    public Guid EventId { get; init; }

    public string EventType { get; init; } = string.Empty;

    public string Payload { get; init; } = string.Empty;

    public string Error { get; init; } = string.Empty;

    public int RetryCount { get; init; }

    public DateTime FailedAt { get; init; } =
        DateTime.UtcNow;
}
