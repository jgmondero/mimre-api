using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Entities;
using Mimre.Domain.Exceptions;

namespace Mimre.Application.Features.ShareLinks.Queries.GetGalleryByToken;

public class GetGalleryByTokenQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetGalleryByTokenQuery, GalleryDto>
{
    public async Task<GalleryDto> Handle(GetGalleryByTokenQuery request, CancellationToken ct)
    {
        var link = await uow.ShareLinks.GetByTokenAsync(request.Token, ct)
            ?? throw new NotFoundException(nameof(ShareLink), request.Token);

        if (link.IsExpired())
            throw new DomainException("This share link has expired.");

        link.IncrementView();
        await uow.SaveChangesAsync(ct);

        var g = link.Gallery;
        return new GalleryDto(
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
}
