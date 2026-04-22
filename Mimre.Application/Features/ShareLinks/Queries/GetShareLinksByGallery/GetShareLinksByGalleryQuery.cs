using MediatR;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.ShareLinks.Queries.GetShareLinksByGallery;

public record GetShareLinksByGalleryQuery(Guid GalleryId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<ShareLinkDto>>;
