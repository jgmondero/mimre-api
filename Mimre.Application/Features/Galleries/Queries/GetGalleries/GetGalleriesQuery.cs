using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleries;

public record GetGalleriesQuery(Guid UserId) : IRequest<IReadOnlyList<GalleryDto>>;
