using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleries;

public class GetGalleriesQueryHandler(IUnitOfWork uow) : IRequestHandler<GetGalleriesQuery, IReadOnlyList<GalleryDto>>
{
    public async Task<IReadOnlyList<GalleryDto>> Handle(GetGalleriesQuery request, CancellationToken ct)
    {
        var galleries = await uow.Galleries.GetByUserIdAsync(request.UserId, ct);
        return galleries
            .Select(g => new GalleryDto(
                g.Id, 
                g.Title, 
                g.Slug, 
                g.IsPublished,
                g.CoverPhotoId, 
                g.ExpiresAt, 
                g.PasswordHash is not null,
                g.CreatedOn, 
                g.ModifiedOn)).ToList();
    }
}
