using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleryById;

public record GetGalleryByIdQuery(
    Guid GalleryId,
    Guid UserId) : IRequest<GalleryDto>;
