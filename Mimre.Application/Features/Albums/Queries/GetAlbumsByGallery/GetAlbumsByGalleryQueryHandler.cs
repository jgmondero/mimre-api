using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.Albums.Queries.GetAlbumsByGallery;

public class GetAlbumsByGalleryQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAlbumsByGalleryQuery, PagedResult<AlbumDto>>
{
    public async Task<PagedResult<AlbumDto>> Handle(GetAlbumsByGalleryQuery request, CancellationToken ct)
    {
        var result = await uow.Albums.GetByGalleryIdAsync(request.GalleryId, request.Page, request.PageSize, ct);

        return new PagedResult<AlbumDto>
        {
            Items = result.Items.Select(a => new AlbumDto(
                a.Id,
                a.GalleryId,
                a.Title,
                a.SortOrder,
                a.Photos.Count)).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
