using MediatR;
using Mimre.Application.Common.Interfaces;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.ShareLinks.Queries.GetShareLinksByGallery;

public class GetShareLinksByGalleryQueryHandler(IUnitOfWork uow)
    : IRequestHandler<GetShareLinksByGalleryQuery, PagedResult<ShareLinkDto>>
{
    public async Task<PagedResult<ShareLinkDto>> Handle(GetShareLinksByGalleryQuery request, CancellationToken ct)
    {
        var result = await uow.ShareLinks.GetByGalleryIdAsync(request.GalleryId, request.Page, request.PageSize, ct);

        return new PagedResult<ShareLinkDto>
        {
            Items = result.Items.Select(l => new ShareLinkDto(
                l.Id,
                l.GalleryId,
                l.Token,
                l.ClientEmail,
                l.DownloadPermission,
                l.ExpiresAt,
                l.ViewCount,
                ShareUrl: $"https://mimre.app/c/{l.Token}")).ToList(),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalCount = result.TotalCount
        };
    }
}
