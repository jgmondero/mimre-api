using MediatR;
using Mimre.Application.DTOs;
using Mimre.Domain.Common;

namespace Mimre.Application.Features.Photos.Queries.GetPhotosByAlbum;

public record GetPhotosByAlbumQuery(Guid AlbumId, Guid? Cursor = null, int PageSize = 30) : IRequest<CursorResult<PhotoDto>>;
