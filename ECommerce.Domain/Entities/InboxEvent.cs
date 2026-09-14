namespace ECommerce.Domain.Entities;

public class InboxEvent
{
    public Guid EventId { get; private set; }

    public string EventType { get; private set; }

    public DateTime ProcessedAt { get; private set; }

    private InboxEvent()
    {
    }

    public InboxEvent(
        Guid eventId,
        string eventType)
    {
        if (eventId == Guid.Empty)
        {
            throw new ArgumentException(
                "Event ID is required.");
        }

        if (string.IsNullOrWhiteSpace(eventType))
        {
            throw new ArgumentException(
                "Event type is required.");
        }

        EventId = eventId;
        EventType = eventType;
        ProcessedAt = DateTime.UtcNow;
    }
}