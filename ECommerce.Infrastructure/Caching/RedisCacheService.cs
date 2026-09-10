using System.Text.Json;
using ECommerce.Application.Interfaces;
using StackExchange.Redis;

namespace ECommerce.Infrastructure.Caching;

public class RedisCacheService : ICacheService
{
    private readonly IConnectionMultiplexer _redis;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public RedisCacheService(
        IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<T?> GetAsync<T>(
        string key,
        CancellationToken cancellationToken = default)
    {
        var database = _redis.GetDatabase();

        var value = await database.StringGetAsync(key);

        if (value.IsNullOrEmpty)
        {
            return default;
        }

        return JsonSerializer.Deserialize<T>(
            value.ToString(),
            JsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken cancellationToken = default)
    {
        var database = _redis.GetDatabase();

        var serialized =
            JsonSerializer.Serialize(value, JsonOptions);

        await database.StringSetAsync(
            key,
            serialized,
            expiration);
    }

    public async Task RemoveAsync(
        string key,
        CancellationToken cancellationToken = default)
    {
        var database = _redis.GetDatabase();

        await database.KeyDeleteAsync(key);
    }
}