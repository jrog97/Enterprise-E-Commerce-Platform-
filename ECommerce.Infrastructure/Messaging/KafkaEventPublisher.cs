using System.Text.Json;
using Confluent.Kafka;
using ECommerce.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using ECommerce.Application.Events;

namespace ECommerce.Infrastructure.Messaging;

public class KafkaEventPublisher : IEventPublisher
{
    private readonly IProducer<string, string> _producer;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public KafkaEventPublisher(IConfiguration configuration)
    {
        var bootstrapServers =
            configuration["Kafka:BootstrapServers"]
            ?? "localhost:9092";

        var config = new ProducerConfig
        {
            BootstrapServers = bootstrapServers
        };

        _producer =
            new ProducerBuilder<string, string>(config)
                .Build();
    }

    public async Task PublishAsync<T>(
        string topic,
        T eventMessage,
        CancellationToken cancellationToken = default)
    {
        var message =
            JsonSerializer.Serialize(
                eventMessage,
                JsonOptions);

        await _producer.ProduceAsync(
            topic,
            new Message<string, string>
            {
                Key = eventMessage is OrderCreatedEvent orderCreated
                ? orderCreated.OrderId.ToString()
                : Guid.NewGuid().ToString(),
                Value = message
            },
            cancellationToken);
    }
}