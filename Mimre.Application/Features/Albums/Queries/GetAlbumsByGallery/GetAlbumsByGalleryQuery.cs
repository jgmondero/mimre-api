using MediatR;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.Albums.Queries.GetAlbumsByGallery;

public record GetAlbumsByGalleryQuery(Guid GalleryId, int Page = 1, int PageSize = 20) : IRequest<PagedResult<AlbumDto>>;
