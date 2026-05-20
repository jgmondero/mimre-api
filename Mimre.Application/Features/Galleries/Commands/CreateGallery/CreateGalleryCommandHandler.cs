using MediatR;
using Microsoft.Extensions.Logging;
using Mimre.Application.Common.Constants;
using Mimre.Application.Common.Helpers;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Commands.CreateGallery;

public class CreateGalleryCommandHandler(IUnitOfWork uow, ICacheService cache, ILogger<CreateGalleryCommandHandler> logger) : IRequestHandler<CreateGalleryCommand, GalleryDto>
{
    public async Task<GalleryDto> Handle(CreateGalleryCommand request, CancellationToken ct)
    {
        var slug = SlugHelper.GenerateSlug(request.Title);
        if (await uow.Galleries.SlugExistsAsync(request.UserId, slug, ct))
            throw new DomainException($"Could not generate a unique slug for '{request.Title}'. Please try a different title.");

        var gallery = Gallery.Create(request.UserId, request.Title, slug);
        uow.Galleries.Add(gallery);
        await uow.SaveChangesAsync(ct);

        await cache.RemoveByPrefixAsync(CacheKeys.UserGalleriesPrefix(request.UserId), ct);

        logger.LogInformation("Gallery created. {GalleryId} {Slug} {UserId}", gallery.Id, gallery.Slug, request.UserId);

        return ToDto(gallery);
    }

    private static GalleryDto ToDto(Gallery g) => new(
        g.Id, 
        g.Title, 
        g.Slug, 
        g.IsPublished,
        g.CoverPhotoId, 
        g.ExpiresAt, 
        g.PasswordHash is not null,
        g.CreatedOn, 
        g.ModifiedOn);
}
