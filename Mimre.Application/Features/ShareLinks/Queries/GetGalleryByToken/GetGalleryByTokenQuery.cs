using MediatR;
using Mimre.Application.DTOs;

namespace Mimre.Application.Features.ShareLinks.Queries.GetGalleryByToken;

public record GetGalleryByTokenQuery(string Token) : IRequest<GalleryDto>;
