using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;
using Mimre.Domain.Interfaces.Services;
using System.Security.Cryptography.X509Certificates;

namespace Mimre.Application.Features.Photos.Queries.GetPhotosByAlbum;

public class GetPhotosByAlbumQueryHandler(IUnitOfWork uow, IBlobStorageService blob)
    : IRequestHandler<GetPhotosByAlbumQuery, CursorResult<PhotoDto>>
{
    public async Task<CursorResult<PhotoDto>> Handle(GetPhotosByAlbumQuery request, CancellationToken ct)
    {
        var photos = await uow.Photos.GetByAlbumIdAsync(request.AlbumId, request.Cursor, request.PageSize, ct);
        var result = new List<PhotoDto>();

        foreach (var photo in photos.Items)
        {
            // Thumbnails are public CDN URLs, originals are SAS-protected
            var thumbnailUrl = photo.ThumbnailBlobPath is not null
                ? await blob.GenerateSasUrlAsync(photo.ThumbnailBlobPath, TimeSpan.FromHours(1))
                : string.Empty;

            var watermarkedUrl = photo.WatermarkedBlobPath is not null
                ? await blob.GenerateSasUrlAsync(photo.WatermarkedBlobPath, TimeSpan.FromHours(1))
                : null;

            result.Add(new PhotoDto(
                photo.Id,
                photo.AlbumId,
                photo.OriginalFileName,
                thumbnailUrl,
                watermarkedUrl,
                photo.FileSizeBytes,
                photo.Width,
                photo.Height,
                photo.SortOrder,
                photo.Status));
        }

        return new CursorResult<PhotoDto>
        {
            Items = result,
            NextCursor = photos.NextCursor,
            PageSize = request.PageSize
        };
    }
}
