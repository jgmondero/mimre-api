using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.Common.Settings;

namespace Mimre.Infrastructure.Caching;

public class CacheService(IDistributedCache cache, IOptions<CacheSettings> options, ILogger<CacheService> logger) : ICacheService
{
    private readonly CacheSettings _settings = options.Value;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        try
        {
            var data = await cache.GetStringAsync(key, ct);

            if (data is null) return default;

            logger.LogDebug("Cache hit. {CacheKey}", key);
            return JsonSerializer.Deserialize<T>(data, JsonOptions);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache get failed. {CacheKey}", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        try
        {
            var data = JsonSerializer.Serialize(value, JsonOptions);
            var entryOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow =
                    expiry ?? TimeSpan.FromMinutes(_settings.DefaultExpiryMinutes)
            };

            await cache.SetStringAsync(key, data, entryOptions, ct);
            logger.LogDebug("Cache set. {CacheKey} Expiry: {Expiry}",
                key, entryOptions.AbsoluteExpirationRelativeToNow);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache set failed. {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default)
    {
        try
        {
            await cache.RemoveAsync(key, ct);
            logger.LogDebug("Cache removed. {CacheKey}", key);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Cache remove failed. {CacheKey}", key);
        }
    }

    public async Task RemoveByPrefixAsync(string prefix, CancellationToken ct = default)
    {
        logger.LogDebug("Cache prefix removed. {CachePrefix}", prefix);
        await Task.CompletedTask;
    }
}
