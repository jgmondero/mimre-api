using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Interfaces.Repositories;
using Mimre.Domain.Interfaces.Services;
using Mimre.Infrastructure.Auth;
using Mimre.Infrastructure.BlobStorage;
using Mimre.Infrastructure.Caching;
using Mimre.Infrastructure.ImageProcessing;
using Mimre.Infrastructure.Persistence;
using Mimre.Infrastructure.Persistence.Repositories;
using Mimre.Infrastructure.Queue;
using Mimre.Application.Common.Settings;

namespace Mimre.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<MimreDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGalleryRepository, GalleryRepository>();
        services.AddScoped<IAlbumRepository, AlbumRepository>();
        services.AddScoped<IPhotoRepository, PhotoRepository>();
        services.AddScoped<IShareLinkRepository, ShareLinkRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Auth
        services.Configure<JwtSettings>(configuration.GetSection(nameof(JwtSettings)));
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();

        // Blob Storage
        services.Configure<BlobStorageSettings>(configuration.GetSection(nameof(BlobStorageSettings)));
        services.AddScoped<IBlobStorageService, AzureBlobStorageService>();

        // Image Processing
        services.AddScoped<IImageProcessingService, SkiaImageProcessingService>();

        // Queue
        services.Configure<QueueSettings>(configuration.GetSection(nameof(QueueSettings)));
        services.AddScoped<IStorageQueueService, AzureStorageQueueService>();

        // Caching
        services.Configure<CacheSettings>(configuration.GetSection(nameof(CacheSettings)));

        var cacheSettings = configuration
            .GetSection(nameof(CacheSettings))
            .Get<CacheSettings>() ?? new CacheSettings();

        if (cacheSettings.UseRedis && !string.IsNullOrEmpty(cacheSettings.RedisConnectionString))
        {
            // Production — Redis distributed cache
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = cacheSettings.RedisConnectionString;
                options.InstanceName = "mimre:";
            });
            services.AddScoped<ICacheService, CacheService>();
        }
        else
        {
            // Development — in-memory cache
            services.AddMemoryCache();
            services.AddScoped<ICacheService, MemoryCacheService>();
        }

        return services;
    }
}
