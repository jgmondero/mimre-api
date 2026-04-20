using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Albums.Commands.UpdateAlbum;

public record UpdateAlbumCommand(
    [property: JsonIgnore] Guid AlbumId,
    string Title,
    int SortOrder) : IRequest;
