namespace ECommerce.Application.Interfaces;

public interface IEventHandler<in TEvent>
{
    Task HandleAsync(
        TEvent eventMessage,
        CancellationToken cancellationToken = default);
}