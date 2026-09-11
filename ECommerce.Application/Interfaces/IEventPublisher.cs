namespace ECommerce.Application.Interfaces;

public interface IEventPublisher
{
    Task PublishAsync<T>(
        string topic,
        T eventMessage,
        CancellationToken cancellationToken = default);
}