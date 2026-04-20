using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Albums.Commands.CreateAlbum;

public record CreateAlbumCommand(
    Guid GalleryId,
    string Title,
    int SortOrder = 0) : IRequest<AlbumDto>;
