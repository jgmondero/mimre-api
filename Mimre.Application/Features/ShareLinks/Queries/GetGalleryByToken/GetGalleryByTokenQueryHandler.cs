using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Application.Common.Settings;
using Microsoft.Extensions.Options;

namespace Mimre.Application.Features.ShareLinks.Queries.GetGalleryByToken;

public class GetGalleryByTokenQueryHandler(IUnitOfWork uow, ICacheService cache, IOptions<CacheSettings> cacheSettings, ILogger<GetGalleryByTokenQueryHandler> logger)
    : IRequestHandler<GetGalleryByTokenQuery, GalleryDto>
{
    public async Task<GalleryDto> Handle(GetGalleryByTokenQuery request, CancellationToken ct)
    {
        var cacheKey = CacheKeys.GalleryByToken(request.Token);

        var cached = await cache.GetAsync<GalleryDto>(cacheKey, ct);
        if (cached is not null)
            return cached;

        var link = await uow.ShareLinks.GetByTokenAsync(request.Token, ct)
            ?? throw new NotFoundException(nameof(ShareLink), request.Token);

        if (link.IsExpired())
            throw new DomainException("This share link has expired.");

        link.IncrementView();
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Gallery accessed via share link. {Token} {GalleryId} ViewCount: {ViewCount}", request.Token, link.GalleryId, link.ViewCount);

        var g = link.Gallery;
        var result = new GalleryDto(
            g.Id,
            g.Title,
            g.Slug,
            g.IsPublished,
            g.CoverPhotoId,
            g.ExpiresAt,
            g.PasswordHash is not null,
            g.CreatedOn,
            g.ModifiedOn);

        // Store in cache
        await cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(cacheSettings.Value.GalleryTokenExpiryMinutes),
            ct);

        return result;
    }

    

}
