using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleryById;

public class GetGalleryByIdQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetGalleryByIdQuery, GalleryDto>
{
    public async Task<GalleryDto> Handle(GetGalleryByIdQuery request, CancellationToken ct)
    {
        var gallery = await uow.Galleries.GetByIdAsync(request.GalleryId, ct)
            ?? throw new NotFoundException(nameof(Gallery), request.GalleryId);

        if (gallery.UserId != request.UserId)
            throw new DomainException("Access denied.");

        return new GalleryDto(
            gallery.Id, gallery.Title, gallery.Slug, gallery.IsPublished,
            gallery.CoverPhotoId, gallery.ExpiresAt, gallery.PasswordHash is not null,
            gallery.CreatedOn, gallery.ModifiedOn);
    }
}
