using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace Mimre.Application.Features.Photos.Commands.SetCoverPhoto;

public class SetCoverPhotoCommandHandler(
    IUnitOfWork uow,
    ILogger<SetCoverPhotoCommandHandler> logger)
    : IRequestHandler<SetCoverPhotoCommand>
{
    public async Task Handle(SetCoverPhotoCommand request, CancellationToken ct)
    {
        var photo = await uow.Photos.GetByIdAsync(request.PhotoId, ct)
            ?? throw new NotFoundException(nameof(Photo), request.PhotoId);

        // Verify ownership through album → gallery chain
        var gallery = await uow.Galleries.GetByIdAsync(photo.Album.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), photo.Album.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        // Photo must be processed before setting as cover
        if (photo.Status != Domain.Enums.PhotoStatus.Ready)
            throw new DomainException("Photo must be fully processed before setting as cover.");

        gallery.SetCoverPhoto(photo.Id);
        await uow.SaveChangesAsync(ct);

        logger.LogInformation("Cover photo set. {GalleryId} {PhotoId}",
            gallery.Id, photo.Id);
    }
}
