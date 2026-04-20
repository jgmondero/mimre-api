using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Albums.Queries.GetAlbumsByGallery;

public class GetAlbumsByGalleryQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetAlbumsByGalleryQuery, IReadOnlyList<AlbumDto>>
{
    public async Task<IReadOnlyList<AlbumDto>> Handle(GetAlbumsByGalleryQuery request, CancellationToken ct)
    {
        var albums = await uow.Albums.GetByGalleryIdAsync(request.GalleryId, ct);

        return albums.Select(a => new AlbumDto(
            a.Id,
            a.GalleryId,
            a.Title,
            a.SortOrder,
            PhotoCount: a.Photos.Count)).ToList();
    }
}
