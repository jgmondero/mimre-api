using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Mimre.Domain.Interfaces.Services;

namespace Mimre.Application.Features.Photos.Commands.DeletePhoto;

public class DeletePhotoCommandHandler(IUnitOfWork uow, IBlobStorageService blob, ICacheService cache, ILogger<DeletePhotoCommandHandler> logger)
    : IRequestHandler<DeletePhotoCommand>
{
    public async Task Handle(DeletePhotoCommand request, CancellationToken ct)
    {
        var photo = await uow.Photos.GetByIdAsync(request.PhotoId, ct)
            ?? throw new NotFoundException(nameof(Photo), request.PhotoId);

        // Remove all blob variants
        await blob.DeleteAsync(photo.BlobPath, ct);

        if (photo.ThumbnailBlobPath is not null)
            await blob.DeleteAsync(photo.ThumbnailBlobPath, ct);

        if (photo.WatermarkedBlobPath is not null)
            await blob.DeleteAsync(photo.WatermarkedBlobPath, ct);

        // Update user storage quota
        var user = await uow.Users.GetByIdAsync(
            photo.Album.Gallery.UserId, ct)
            ?? throw new NotFoundException(nameof(User), photo.Album.Gallery.UserId);

        user.AddStorageUsage(-photo.FileSizeBytes);

        uow.Photos.Remove(photo);
        await uow.SaveChangesAsync(ct);

        await cache.RemoveByPrefixAsync(CacheKeys.AlbumPhotosPrefix(photo.AlbumId), ct);

        logger.LogInformation("Photo deleted. {PhotoId} {BlobPath}", photo.Id, photo.BlobPath);
    }
}
