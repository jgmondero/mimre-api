using MediatR;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Galleries.Commands.UpdateGallery;

public record UpdateGalleryCommand(
    [property: JsonIgnore] Guid GalleryId,
    [property: JsonIgnore] Guid UserId,
    string Title) : IRequest;
