using MediatR;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleries;

public record GetGalleriesQuery(Guid UserId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<GalleryDto>>;
