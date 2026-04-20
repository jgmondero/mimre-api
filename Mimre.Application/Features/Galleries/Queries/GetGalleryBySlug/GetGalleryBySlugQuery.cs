using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.Galleries.Queries.GetGalleryBySlug;

public record GetGalleryBySlugQuery(Guid UserId, string Slug) : IRequest<GalleryDto>;
