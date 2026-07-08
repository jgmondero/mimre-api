using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Albums.Commands.DeleteAlbum;

public record DeleteAlbumCommand(Guid AlbumId, [property: JsonIgnore] Guid UserId = default) : IRequest;
