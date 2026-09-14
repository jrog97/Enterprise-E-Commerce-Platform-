using System.Text.Json;
using ECommerce.Application.Events;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging;

public class OutboxProcessor : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IEventPublisher _eventPublisher;
    private readonly ILogger<OutboxProcessor> _logger;

    private const int BatchSize = 20;

    public OutboxProcessor(
        IServiceScopeFactory scopeFactory,
        IEventPublisher eventPublisher,
        ILogger<OutboxProcessor> logger)
    {
        _scopeFactory = scopeFactory;
        _eventPublisher = eventPublisher;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Outbox processor started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessBatchAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error while processing outbox events.");
            }

            await Task.Delay(
                TimeSpan.FromSeconds(2),
                stoppingToken);
        }

        _logger.LogInformation(
            "Outbox processor stopped.");
    }

    private async Task ProcessBatchAsync(
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var outboxRepository =
            scope.ServiceProvider
                .GetRequiredService<IOutboxRepository>();

        var events =
            await outboxRepository.GetUnprocessedAsync(
                BatchSize,
                cancellationToken);

        if (events.Count == 0)
        {
            return;
        }

        foreach (var outboxEvent in events)
        {
            try
            {
                await PublishEventAsync(
                    outboxEvent,
                    cancellationToken);

                outboxEvent.MarkAsProcessed();

                _logger.LogInformation(
                    "Published outbox event {EventId}. " +
                    "Type: {EventType}",
                    outboxEvent.Id,
                    outboxEvent.EventType);
            }
            catch (Exception ex)
            {
                outboxEvent.MarkAsFailed(
                    ex.Message);

                _logger.LogError(
                    ex,
                    "Failed to publish outbox event {EventId}.",
                    outboxEvent.Id);
            }
        }

        await outboxRepository.SaveChangesAsync(
            cancellationToken);
    }

    private async Task PublishEventAsync(
        ECommerce.Domain.Entities.OutboxEvent outboxEvent,
        CancellationToken cancellationToken)
    {
        switch (outboxEvent.EventType)
        {
            case "OrderCreated":
            {
                var eventMessage =
                    JsonSerializer.Deserialize<OrderCreatedEvent>(
                        outboxEvent.Payload);

                if (eventMessage == null)
                {
                    throw new InvalidOperationException(
                        "Could not deserialize OrderCreated event.");
                }

                await _eventPublisher.PublishAsync(
                    "order-created",
                    eventMessage,
                    cancellationToken);

                break;
            }

            default:
                throw new InvalidOperationException(
                    $"Unknown event type: {outboxEvent.EventType}");
        }
    }
}