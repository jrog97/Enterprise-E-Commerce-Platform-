using System.Text.Json;
using Confluent.Kafka;
using ECommerce.Application.Events;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private const int MaxProcessingAttempts = 3;

    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedConsumer> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    private IConsumer<string, string>? _consumer;

    public OrderCreatedConsumer(
        IConfiguration configuration,
        ILogger<OrderCreatedConsumer> logger,
        IServiceScopeFactory scopeFactory)
    {
        _configuration = configuration;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var bootstrapServers =
            _configuration["Kafka:BootstrapServers"]
            ?? "localhost:9092";

        var config = new ConsumerConfig
        {
            BootstrapServers = bootstrapServers,
            GroupId = "ecommerce-order-created-consumer",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false
        };

        _consumer =
            new ConsumerBuilder<string, string>(config)
                .Build();

        _consumer.Subscribe("order-created");

        _logger.LogInformation(
            "OrderCreatedConsumer started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result =
                        _consumer.Consume(stoppingToken);

                    var eventMessage =
                        JsonSerializer.Deserialize<OrderCreatedEvent>(
                            result.Message.Value);

                    if (eventMessage is null)
                    {
                        _logger.LogWarning(
                            "Received invalid OrderCreated event.");

                        _consumer.Commit(result);

                        continue;
                    }

                    await ProcessMessageAsync(
                        result,
                        eventMessage,
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
                        "Unexpected error in OrderCreatedConsumer.");
                }
            }
        }
        finally
        {
            _consumer.Close();

            _logger.LogInformation(
                "OrderCreatedConsumer stopped.");
        }
    }

    private async Task ProcessMessageAsync(
        ConsumeResult<string, string> result,
        OrderCreatedEvent eventMessage,
        CancellationToken cancellationToken)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var inboxService =
            scope.ServiceProvider
                .GetRequiredService<IInboxService>();

        for (
            var attempt = 1;
            attempt <= MaxProcessingAttempts;
            attempt++)
        {
            try
            {
                _logger.LogInformation(
                    "Processing OrderCreated event. " +
                    "EventId: {EventId}, OrderId: {OrderId}, " +
                    "Attempt: {Attempt}/{MaxAttempts}",
                    eventMessage.EventId,
                    eventMessage.OrderId,
                    attempt,
                    MaxProcessingAttempts);

                var processed =
                    await inboxService.ProcessAsync(
                        eventMessage,
                        cancellationToken);

                if (!processed)
                {
                    _logger.LogInformation(
                        "Duplicate OrderCreated event detected. " +
                        "EventId: {EventId}",
                        eventMessage.EventId);
                }

                _consumer!.Commit(result);

                _logger.LogInformation(
                    "Kafka offset committed. " +
                    "EventId: {EventId}, OrderId: {OrderId}",
                    eventMessage.EventId,
                    eventMessage.OrderId);

                return;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(
                    ex,
                    "Failed to process OrderCreated event. " +
                    "EventId: {EventId}, Attempt: {Attempt}/{MaxAttempts}",
                    eventMessage.EventId,
                    attempt,
                    MaxProcessingAttempts);

                if (attempt == MaxProcessingAttempts)
                {
                    await MoveToDeadLetterQueueAsync(
                        result.Message.Value,
                        eventMessage.EventId,
                        "OrderCreated",
                        ex.Message,
                        attempt,
                        cancellationToken);

                    _consumer!.Commit(result);

                    _logger.LogError(
                        "OrderCreated event moved to DLQ. " +
                        "EventId: {EventId}, OrderId: {OrderId}",
                        eventMessage.EventId,
                        eventMessage.OrderId);

                    return;
                }

                var delaySeconds =
                    Math.Pow(2, attempt);

                await Task.Delay(
                    TimeSpan.FromSeconds(delaySeconds),
                    cancellationToken);
            }
        }
    }

    private async Task MoveToDeadLetterQueueAsync(
        string payload,
        Guid eventId,
        string eventType,
        string error,
        int attempt,
        CancellationToken cancellationToken)
    {
        // DLQ implementation will go here.
        // For now, log the failed event.

        _logger.LogError(
            "Dead-lettering event. " +
            "EventId: {EventId}, EventType: {EventType}, " +
            "Attempt: {Attempt}, Error: {Error}, Payload: {Payload}",
            eventId,
            eventType,
            attempt,
            error,
            payload);

        await Task.CompletedTask;
    }
}