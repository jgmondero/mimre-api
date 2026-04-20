using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Photos.Queries.GetPhotosByAlbum;

public record GetPhotosByAlbumQuery(Guid AlbumId) : IRequest<IReadOnlyList<PhotoDto>>;
