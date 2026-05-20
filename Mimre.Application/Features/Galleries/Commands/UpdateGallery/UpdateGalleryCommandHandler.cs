using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Helpers;
using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Commands.UpdateGallery;

public class UpdateGalleryCommandHandler(IUnitOfWork uow, ICacheService cache, ILogger<UpdateGalleryCommandHandler> logger) : IRequestHandler<UpdateGalleryCommand>
{
    public async Task Handle(UpdateGalleryCommand request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        if(!gallery.IsPublished)
        {
            var newSlug = SlugHelper.GenerateSlug(request.Title);

            if(newSlug != gallery.Slug)
            {
                if(await uow.Galleries.SlugExistsAsync(request.UserId, newSlug, ct))
                    throw new DomainException($"Could not generate a unique slug for '{request.Title}'. Please try a different title.");
            }

            gallery.UpdateSlug(newSlug);
        }

        gallery.UpdateTitle(request.Title);
        await uow.SaveChangesAsync(ct);

        await cache.RemoveByPrefixAsync(CacheKeys.UserGalleriesPrefix(gallery.UserId), ct);
        await cache.RemoveByPrefixAsync("gallery:token:", ct);

        logger.LogInformation("Gallery updated. {GalleryId} {UserId}", request.GalleryId, request.UserId);
    }
}
