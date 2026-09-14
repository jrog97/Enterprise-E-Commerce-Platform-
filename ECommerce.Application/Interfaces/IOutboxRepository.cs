using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces;

public interface IOutboxRepository
{
    Task AddAsync(
        OutboxEvent outboxEvent,
        CancellationToken cancellationToken = default);

    Task<List<OutboxEvent>> GetUnprocessedAsync(
        int batchSize,
        CancellationToken cancellationToken = default);

    Task SaveChangesAsync(
        CancellationToken cancellationToken = default);
}