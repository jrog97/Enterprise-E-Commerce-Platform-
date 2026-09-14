using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IInboxRepository
{
    Task<bool> ExistsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default);

    Task AddAsync(
        InboxEvent inboxEvent,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}