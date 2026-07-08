using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Photos.Commands.DeletePhoto;

public record DeletePhotoCommand(Guid PhotoId, [property: JsonIgnore] Guid UserId = default) : IRequest;
