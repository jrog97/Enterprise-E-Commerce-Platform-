using ECommerce.Application.Events;

namespace ECommerce.Application.Interfaces;

public interface IInboxService
{
    Task<bool> ProcessAsync(
        OrderCreatedEvent eventMessage,
        CancellationToken cancellationToken = default);
}