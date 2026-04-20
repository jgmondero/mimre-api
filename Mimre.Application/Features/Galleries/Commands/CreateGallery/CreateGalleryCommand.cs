using MediatR;
using Mimre.Application.DTOs;
using System.Text.Json.Serialization;

namespace Mimre.Application.Features.Galleries.Commands.CreateGallery;

public record CreateGalleryCommand(
    [property: JsonIgnore] Guid UserId,
    string Title) : IRequest<GalleryDto>;
