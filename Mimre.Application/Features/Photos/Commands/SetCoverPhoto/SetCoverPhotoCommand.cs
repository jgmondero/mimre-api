using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Photos.Commands.SetCoverPhoto;

public record SetCoverPhotoCommand(
    [property: JsonIgnore] Guid PhotoId = default,
    [property: JsonIgnore] Guid UserId = default) : IRequest;
