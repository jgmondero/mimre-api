using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Albums.Commands.CreateAlbum;

public class CreateAlbumCommandHandler(IUnitOfWork uow, ICacheService cache, ILogger<CreateAlbumCommandHandler> logger) : IRequestHandler<CreateAlbumCommand, AlbumDto>
{
    public async Task<AlbumDto> Handle(CreateAlbumCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        var album = Album.Create(gallery.Id, request.Title, request.SortOrder);
        uow.Albums.Add(album);
        await uow.SaveChangesAsync(ct);

        await cache.RemoveByPrefixAsync(CacheKeys.GalleryAlbumsPrefix(request.GalleryId), ct);

        logger.LogInformation("Album created. {AlbumId} {GalleryId}", album.Id, album.GalleryId);

        return new AlbumDto(album.Id, album.GalleryId, album.Title, album.SortOrder, PhotoCount: 0);
    }
}
