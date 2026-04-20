using MediatR;
using Mimre.Application.Common.Helpers;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Commands.CreateGallery;

public class CreateGalleryCommandHandler(IUnitOfWork uow) : IRequestHandler<CreateGalleryCommand, GalleryDto>
{
    public async Task<GalleryDto> Handle(CreateGalleryCommand request, CancellationToken ct)
    {
        var slug = SlugHelper.GenerateSlug(request.Title);
        if (await uow.Galleries.SlugExistsAsync(request.UserId, slug, ct))
            throw new DomainException($"Could not generate a unique slug for '{request.Title}'. Please try a different title.");

        var gallery = Gallery.Create(request.UserId, request.Title, slug);
        uow.Galleries.Add(gallery);
        await uow.SaveChangesAsync(ct);

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
