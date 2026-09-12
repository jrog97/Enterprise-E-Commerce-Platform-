using System.Text.Json;
using Confluent.Kafka;
using ECommerce.Application.Events;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;


namespace ECommerce.Infrastructure.Messaging.Consumers;

public class OrderCreatedConsumer : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OrderCreatedConsumer> _logger;

    private IConsumer<string, string>? _consumer;

    public OrderCreatedConsumer(
        IConfiguration configuration,
        ILogger<OrderCreatedConsumer> logger)
    {
        _configuration = configuration;
        _logger = logger;
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
