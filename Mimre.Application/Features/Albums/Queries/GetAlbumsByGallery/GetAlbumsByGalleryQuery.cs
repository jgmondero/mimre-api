using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Albums.Queries.GetAlbumsByGallery;

public record GetAlbumsByGalleryQuery(Guid GalleryId) : IRequest<IReadOnlyList<AlbumDto>>;
