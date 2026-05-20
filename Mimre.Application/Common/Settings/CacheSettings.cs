namespace Mimre.Application.Common.Settings;

public class CacheSettings
{
    public bool UseRedis { get; init; } = false;
    public string? RedisConnectionString { get; init; }
    public int DefaultExpiryMinutes { get; init; } = 10;
    public int GalleryTokenExpiryMinutes { get; init; } = 30;
    public int PhotoListExpiryMinutes { get; init; } = 5;
}
