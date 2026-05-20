using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Commands.DeleteGallery;

public class DeleteGalleryCommandHandler(IUnitOfWork uow, ICacheService cache, ILogger<DeleteGalleryCommandHandler> logger) : IRequestHandler<DeleteGalleryCommand>
{
    public async Task Handle(DeleteGalleryCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        uow.Galleries.Remove(gallery);
        await uow.SaveChangesAsync(ct);

        await cache.RemoveByPrefixAsync(CacheKeys.UserGalleriesPrefix(gallery.UserId), ct);

        logger.LogInformation("Gallery deleted. {GalleryId} {UserId}", request.GalleryId, request.UserId);
    }
}
