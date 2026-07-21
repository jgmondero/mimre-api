using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Galleries.Commands.SetGalleryPassword;

public record SetGalleryPasswordCommand(
    string? Password,
    [property: JsonIgnore] Guid GalleryId = default,
    [property: JsonIgnore] Guid UserId = default) : IRequest;
