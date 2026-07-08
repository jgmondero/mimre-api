using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Albums.Commands.UpdateAlbum;

public class UpdateAlbumCommandHandler(IUnitOfWork uow, ICacheService cache, ILogger<UpdateAlbumCommandHandler> logger) : IRequestHandler<UpdateAlbumCommand>
{
    public async Task Handle(UpdateAlbumCommand request, CancellationToken ct)
    {
        var album = await uow.Albums.GetByIdAsync(request.AlbumId, ct)
            ?? throw new NotFoundException(nameof(Album), request.AlbumId);

        var gallery = await uow.Galleries.GetByIdAsync(album.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), album.GalleryId);

        if (gallery?.UserId != request.UserId)
            throw new UnauthorizedAccessException("Access denied.");

        album.Rename(request.Title);
        album.Reorder(request.SortOrder);
        await uow.SaveChangesAsync(ct);

        await cache.RemoveByPrefixAsync(CacheKeys.GalleryAlbumsPrefix(album.GalleryId), ct);

        logger.LogInformation("Album updated. {AlbumId}", request.AlbumId);
    }
}
