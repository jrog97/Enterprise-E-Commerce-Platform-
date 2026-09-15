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

        using var consumer =
            new ConsumerBuilder<string, string>(config)
                .Build();

        consumer.Subscribe("order-created");

        _logger.LogInformation(
            "OrderCreatedConsumer started.");

        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var result =
                        consumer.Consume(stoppingToken);

                    var orderCreatedEvent =
                        JsonSerializer.Deserialize<OrderCreatedEvent>(
                            result.Message.Value);

                    if (orderCreatedEvent is null)
                    {
                        _logger.LogWarning(
                            "Received invalid OrderCreated event.");

                        consumer.Commit(result);
                        continue;
                    }

                    using var scope =
                        _scopeFactory.CreateScope();

                    var inboxRepository =
                        scope.ServiceProvider
                            .GetRequiredService<IInboxRepository>();

                    var inboxEvent =
                        new InboxEvent(
                            orderCreatedEvent.EventId,
                            "OrderCreated");

                    await inboxRepository.AddAsync(
                        inboxEvent,
                        stoppingToken);

                    await inboxRepository.SaveChangesAsync(
                        stoppingToken);

                    consumer.Commit(result);

                    _logger.LogInformation(
                        "Processed OrderCreated event for OrderId {OrderId}.",
                        orderCreatedEvent.OrderId);
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
                        "Error processing OrderCreated event.");
                }
            }
        }
        finally
        {
            consumer.Close();

            _logger.LogInformation(
                "OrderCreatedConsumer stopped.");
        }
    }
}