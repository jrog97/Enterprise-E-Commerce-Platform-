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
    private const int MaxRetries = 5;

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
                await ProcessBatchAsync(
                    stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Unexpected error in outbox processor.");
            }

            try
            {
                await Task.Delay(
                    TimeSpan.FromSeconds(2),
                    stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
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

        foreach (var outboxEvent in events)
        {
            try
            {
                await PublishEventAsync(
                    outboxEvent,
                    cancellationToken);

                outboxEvent.MarkAsProcessed();

                _logger.LogInformation(
                    "Outbox event {EventId} published successfully.",
                    outboxEvent.Id);
            }
            catch (Exception ex)
            {
                await HandleFailureAsync(
                    outboxEvent,
                    ex,
                    cancellationToken);
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

    private async Task HandleFailureAsync(
        ECommerce.Domain.Entities.OutboxEvent outboxEvent,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (outboxEvent.RetryCount >= MaxRetries)
        {
            await PublishToDeadLetterQueueAsync(
                outboxEvent,
                exception,
                cancellationToken);

            outboxEvent.MoveToDeadLetterQueue(
            exception.Message);

            _logger.LogError(
                exception,
                "Outbox event {EventId} exceeded maximum retries. " +
                "Moved to dead-letter queue.",
                outboxEvent.Id);

            return;
        }

        var retryNumber =
            outboxEvent.RetryCount + 1;

        var delaySeconds =
            Math.Min(
                Math.Pow(2, retryNumber),
                60);

        var nextAttempt =
            DateTime.UtcNow.AddSeconds(
                delaySeconds);

       outboxEvent.MarkAsFailed(
            exception.Message);

        _logger.LogWarning(
            exception,
            "Outbox event {EventId} failed. " +
            "Retry #{RetryNumber} scheduled for {NextAttempt}.",
            outboxEvent.Id,
            retryNumber,
            nextAttempt);
    }

    private async Task PublishToDeadLetterQueueAsync(
        ECommerce.Domain.Entities.OutboxEvent outboxEvent,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var deadLetterEvent =
            new DeadLetterEvent
            {
                EventId = outboxEvent.Id,
                EventType = outboxEvent.EventType,
                Payload = outboxEvent.Payload,
                Error = exception.Message,
                RetryCount = outboxEvent.RetryCount,
                FailedAt = DateTime.UtcNow
            };

        await _eventPublisher.PublishAsync(
            "order-created-dlq",
            deadLetterEvent,
            cancellationToken);
    }
}