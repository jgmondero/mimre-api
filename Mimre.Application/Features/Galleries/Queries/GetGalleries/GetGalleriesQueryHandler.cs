using MediatR;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleries;

public class GetGalleriesQueryHandler(IUnitOfWork uow, ICacheService cache) : IRequestHandler<GetGalleriesQuery, PagedResult<GalleryDto>>
{
    public async Task<PagedResult<GalleryDto>> Handle(GetGalleriesQuery request, CancellationToken ct)
    {
        var cacheKey = CacheKeys.GalleriesByUser(request.UserId, request.Page, request.PageSize);

        var cached = await cache.GetAsync<PagedResult<GalleryDto>>(cacheKey, ct);
        if (cached is not null)
            return cached;

        var result = await uow.Galleries.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, ct);

        var dto = new PagedResult<GalleryDto>
        {
            Items = result.Items.Select(g => new GalleryDto(
                g.Id,
                g.Title,
                g.Slug,
                g.IsPublished,
                g.CoverPhotoId,
                g.ExpiresAt,
                g.PasswordHash is not null,
                g.CreatedOn,
                g.ModifiedOn)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };

        // Store in cache
        await cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(10), ct);
        return dto;
    }
}
