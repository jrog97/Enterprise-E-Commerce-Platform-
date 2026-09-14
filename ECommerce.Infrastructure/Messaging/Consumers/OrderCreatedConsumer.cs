using System.Text.Json;
using Confluent.Kafka;
using ECommerce.Application.Events;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace ECommerce.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
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

                    var orderCreatedEvent =
                        JsonSerializer.Deserialize<OrderCreatedEvent>(
                            result.Message.Value,
                            new JsonSerializerOptions
                            {
                                PropertyNamingPolicy =
                                    JsonNamingPolicy.CamelCase
                            });
                        if (orderCreatedEvent == null)
                                {
                                    _logger.LogWarning(
                                        "Received invalid OrderCreated event.");

                                    continue;
                                }
                        using var scope =
                                _scopeFactory.CreateScope();

                            var inboxRepository =
                                scope.ServiceProvider
                                    .GetRequiredService<IInboxRepository>();

                    if (orderCreatedEvent == null)
                    {
                        _logger.LogWarning(
                            "Received invalid OrderCreated event.");

                        continue;
                    }

                    var alreadyProcessed =
                        await inboxRepository.ExistsAsync(
                            orderCreatedEvent.EventId,
                            stoppingToken);

                    if (alreadyProcessed)
                            {
                                _logger.LogInformation(
                                    "Ignoring duplicate event {EventId}.",
                                    orderCreatedEvent.EventId);

                                _consumer.Commit(result);

                                continue;
                            }
                        var inboxEvent =
                                    new InboxEvent(
                                        orderCreatedEvent.EventId,
                                        "OrderCreated");

                                await inboxRepository.AddAsync(
                                    inboxEvent,
                                    stoppingToken);

                                await inboxRepository.SaveChangesAsync(
                                    stoppingToken);

                                _consumer.Commit(result);

                    _logger.LogInformation(
                        "Processing OrderCreated event. " +
                        "EventId: {EventId}, OrderId: {OrderId}, " +
                        "UserId: {UserId}, Total: {Total}",
                        orderCreatedEvent.EventId,
                        orderCreatedEvent.OrderId,
                        orderCreatedEvent.UserId,
                        orderCreatedEvent.Total);

                    // Inventory processing will be implemented
                    // in a later milestone.

                    _consumer.Commit(result);

                    _logger.LogInformation(
                        "OrderCreated event processed successfully. " +
                        "OrderId: {OrderId}",
                        orderCreatedEvent.OrderId);
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(
                        ex,
                        "Kafka consumption error.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation(
                "OrderCreatedConsumer is stopping.");
        }
        finally
        {
            _consumer.Close();
            _consumer.Dispose();
        }

        await Task.CompletedTask;
    }
}
