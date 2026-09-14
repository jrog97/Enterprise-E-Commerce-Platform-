namespace ECommerce.Domain.Entities;

public class OutboxEvent
{
    public Guid Id { get; private set; }

    public string EventType { get; private set; }

    public string Payload { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? ProcessedAt { get; private set; }

    public DateTime NextAttemptAt { get; private set; }

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
        {
            throw new ArgumentException(
                "Event type is required.");
        }

        if (string.IsNullOrWhiteSpace(payload))
        {
            throw new ArgumentException(
                "Payload is required.");
        }

        Id = Guid.NewGuid();
        EventType = eventType;
        Payload = payload;
        CreatedAt = DateTime.UtcNow;
        NextAttemptAt = DateTime.UtcNow;
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

    var retryDelay = GetRetryDelay(RetryCount);

    NextAttemptAt =
        DateTime.UtcNow.Add(retryDelay);
}

public void MoveToDeadLetterQueue(string error)
{
    RetryCount++;
    Error = $"Moved to DLQ: {error}";

    NextAttemptAt =
        DateTime.UtcNow.AddYears(100);
}

    private static TimeSpan GetRetryDelay(
    int retryCount)
        {
            var seconds =
                Math.Pow(2, retryCount);

            return TimeSpan.FromSeconds(
                Math.Min(seconds, 60));
        }
}