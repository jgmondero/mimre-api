using MediatR;

namespace Mimre.Application.Features.Albums.Commands.DeleteAlbum;

public record DeleteAlbumCommand(Guid AlbumId) : IRequest;
