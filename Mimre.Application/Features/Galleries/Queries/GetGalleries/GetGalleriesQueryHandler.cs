using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleries;

public class GetGalleriesQueryHandler(IUnitOfWork uow) : IRequestHandler<GetGalleriesQuery, PagedResult<GalleryDto>>
{
    public async Task<PagedResult<GalleryDto>> Handle(GetGalleriesQuery request, CancellationToken ct)
    {
        var result = await uow.Galleries.GetByUserIdAsync(request.UserId, request.Page, request.PageSize, ct);

        return new PagedResult<GalleryDto>
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
    }
}
