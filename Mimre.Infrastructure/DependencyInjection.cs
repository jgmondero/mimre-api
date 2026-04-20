using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Interfaces.Repositories;
using Mimre.Domain.Interfaces.Services;
using Mimre.Infrastructure.Auth;
using Mimre.Infrastructure.BlobStorage;
using Mimre.Infrastructure.ImageProcessing;
using Mimre.Infrastructure.Persistence;
using Mimre.Infrastructure.Persistence.Repositories;
using Mimre.Infrastructure.Queue;

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

        return services;
    }
}
