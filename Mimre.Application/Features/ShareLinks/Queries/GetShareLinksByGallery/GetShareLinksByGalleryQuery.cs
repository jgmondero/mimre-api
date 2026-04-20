using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.ShareLinks.Queries.GetShareLinksByGallery;

public record GetShareLinksByGalleryQuery(Guid GalleryId) : IRequest<IReadOnlyList<ShareLinkDto>>;
