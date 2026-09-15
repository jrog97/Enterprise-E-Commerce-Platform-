using ECommerce.Application.Events;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.Services;

public class OrderCreatedEventHandler
    : IEventHandler<OrderCreatedEvent>
{
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(
        ILogger<OrderCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(
        OrderCreatedEvent eventMessage,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Handling OrderCreated event. " +
            "EventId: {EventId}, OrderId: {OrderId}, " +
            "UserId: {UserId}, Total: {Total}",
            eventMessage.EventId,
            eventMessage.OrderId,
            eventMessage.UserId,
            eventMessage.Total);

        // Business processing will be added here later.
        //
        // For example:
        // - Reserve inventory
        // - Trigger fulfillment
        // - Send notification
        // - Update another bounded context

        return Task.CompletedTask;
    }
}