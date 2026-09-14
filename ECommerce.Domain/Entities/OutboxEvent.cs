namespace ECommerce.Domain.Entities;

public class OutboxEvent
{
    public Guid Id { get; private set; }

    public string EventType { get; private set; }

    public string Payload { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    public int RetryCount { get; private set; }

    public string? Error { get; private set; }

    private OutboxEvent()
    {
    }

    public OutboxEvent(
        string eventType,
        string payload)
    {
        if (string.IsNullOrWhiteSpace(eventType))
            throw new ArgumentException(
                "Event type is required.");

        if (string.IsNullOrWhiteSpace(payload))
            throw new ArgumentException(
                "Payload is required.");

        Id = Guid.NewGuid();
        EventType = eventType;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
        RetryCount = 0;
    }

    public void MarkAsProcessed()
    {
        ProcessedAt = DateTime.UtcNow;
        Error = null;
    }

    public void MarkAsFailed(string error)
    {
        RetryCount++;
        Error = error;
    }
}