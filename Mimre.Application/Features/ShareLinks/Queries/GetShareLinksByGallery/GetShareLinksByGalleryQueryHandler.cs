using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.ShareLinks.Queries.GetShareLinksByGallery;

public class GetShareLinksByGalleryQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetShareLinksByGalleryQuery, IReadOnlyList<ShareLinkDto>>
{
    public async Task<IReadOnlyList<ShareLinkDto>> Handle(GetShareLinksByGalleryQuery request, CancellationToken ct)
    {
        var links = await uow.ShareLinks.GetByGalleryIdAsync(request.GalleryId, ct);

        return links.Select(l => new ShareLinkDto(
            l.Id,
            l.GalleryId,
            l.Token,
            l.ClientEmail,
            l.DownloadPermission,
            l.ExpiresAt,
            l.ViewCount,
            ShareUrl: $"https://mimre.app/c/{l.Token}")).ToList();
    }
}
