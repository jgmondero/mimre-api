using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mimre.Application.Common.Interfaces;
using System.Collections.Concurrent;
using System.Text.Json;
using Mimre.Application.Common.Settings;

namespace Mimre.Infrastructure.Caching;

public class MemoryCacheService(IMemoryCache cache, IOptions<CacheSettings> options, ILogger<MemoryCacheService> logger) : ICacheService
{
    private readonly CacheSettings _settings = options.Value;

    private readonly ConcurrentDictionary<string, bool> _keys = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            if (cache.TryGetValue(key, out var cached) && cached is string data)
            {
                logger.LogDebug("Cache hit. {CacheKey}", key);
                return Task.FromResult(JsonSerializer.Deserialize<T>(data, JsonOptions));
            }
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache get failed. {CacheKey}", key);
        }

        return Task.FromResult<T?>(default);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var data = JsonSerializer.Serialize(value, JsonOptions);
            var entryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiry ?? TimeSpan.FromMinutes(_settings.DefaultExpiryMinutes)
            };

            cache.Set(key, data, entryOptions);
            _keys.TryAdd(key, true);
            logger.LogDebug("Cache set. {CacheKey}", key);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache set failed. {CacheKey}", key);
        }

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken ct = default)
    {
        cache.Remove(key);
        _keys.TryRemove(key, out _);
        logger.LogDebug("Cache removed. {CacheKey}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        var keysToRemove = _keys.Keys
            .Where(k => k.StartsWith(prefix))
            .ToList();

        foreach (var key in keysToRemove)
        {
            cache.Remove(key);
            _keys.TryRemove(key, out _);
        }

        logger.LogDebug("Cache prefix removed. {Prefix} KeysRemoved: {Count}",
            prefix, keysToRemove.Count);

        return Task.CompletedTask;
    }
}
