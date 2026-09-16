using ECommerce.Application.Events;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class InboxService : IInboxService
{
    private readonly IInboxRepository _inboxRepository;
    private readonly IEventHandler<OrderCreatedEvent> _eventHandler;
    private readonly IUnitOfWork _unitOfWork;

    private readonly ILogger<InboxService> _logger;

    public InboxService(
        IInboxRepository inboxRepository,
        IEventHandler<OrderCreatedEvent> eventHandler,
        IUnitOfWork unitOfWork,
        ILogger<InboxService> logger)
    {
        _inboxRepository = inboxRepository;
        _eventHandler = eventHandler;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<bool> ProcessAsync(
        OrderCreatedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        var alreadyProcessed =
            await _inboxRepository.ExistsAsync(
                eventMessage.EventId,
                cancellationToken);

        if (alreadyProcessed)
        {
            _logger.LogInformation(
                "Event {EventId} has already been processed.",
                eventMessage.EventId);

            return false;
        }

        await _unitOfWork.BeginTransactionAsync(
            cancellationToken);

        try
        {
            await _eventHandler.HandleAsync(
                eventMessage,
                cancellationToken);

            var inboxEvent =
                new InboxEvent(
                    eventMessage.EventId,
                    "OrderCreated");

            await _inboxRepository.AddAsync(
                inboxEvent,
                cancellationToken);

            await _unitOfWork.SaveChangesAsync(
                cancellationToken);

            await _unitOfWork.CommitTransactionAsync(
                cancellationToken);

            _logger.LogInformation(
                "Event {EventId} committed successfully.",
                eventMessage.EventId);

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(
                cancellationToken);

            throw;
        }
    }
}