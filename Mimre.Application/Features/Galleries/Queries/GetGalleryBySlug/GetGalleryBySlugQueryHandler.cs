using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleryBySlug;

public class GetGalleryBySlugQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetGalleryBySlugQuery, GalleryDto>
{
    public async Task<GalleryDto> Handle(GetGalleryBySlugQuery request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetBySlugAsync(request.UserId, request.Slug, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.Slug);

        return new GalleryDto(
            gallery.Id, 
            gallery.Title, 
            gallery.Slug, 
            gallery.IsPublished,
            gallery.CoverPhotoId, 
            gallery.ExpiresAt, 
            gallery.PasswordHash is not null,
            gallery.CreatedOn, 
            gallery.ModifiedOn);
    }
}
