using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Application.Features.Albums.Commands.DeleteAlbum;

public class DeleteAlbumCommandHandler(IUnitOfWork uow, IBlobStorageService blob, ILogger<DeleteAlbumCommandHandler> logger)
    : IRequestHandler<DeleteAlbumCommand>
{
    public async Task Handle(DeleteAlbumCommand request, CancellationToken ct)
    {
        var album = await uow.Albums.GetByIdAsync(request.AlbumId, ct)
            ?? throw new NotFoundException(nameof(Album), request.AlbumId);

        // Delete all associated blobs before removing DB records
        foreach (var photo in album.Photos)
        {
            await blob.DeleteAsync(photo.BlobPath, ct);

            if (photo.ThumbnailBlobPath is not null)
                await blob.DeleteAsync(photo.ThumbnailBlobPath, ct);

            if (photo.WatermarkedBlobPath is not null)
                await blob.DeleteAsync(photo.WatermarkedBlobPath, ct);
        }

        uow.Albums.Remove(album);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Album deleted. {AlbumId} PhotoCount: {PhotoCount}", request.AlbumId, album.Photos.Count);
    }
}
