using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class InboxRepository : IInboxRepository
{
    private readonly ECommerceDbContext _dbContext;

    public InboxRepository(
        ECommerceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> ExistsAsync(
        Guid eventId,
        CancellationToken cancellationToken = default)
    {
        return await _dbContext.InboxEvents
            .AnyAsync(
                x => x.EventId == eventId,
                cancellationToken);
    }

    public async Task AddAsync(
        InboxEvent inboxEvent,
        CancellationToken cancellationToken = default)
    {
        await _dbContext.InboxEvents.AddAsync(
            inboxEvent,
            cancellationToken);
    }

    public async Task SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(
            cancellationToken);
    }
}