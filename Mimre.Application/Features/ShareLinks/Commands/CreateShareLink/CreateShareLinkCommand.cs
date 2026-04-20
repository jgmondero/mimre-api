using MediatR;
using Mimre.Application.DTOs;
using Mimre.Domain.Enums;

namespace Mimre.Application.Features.ShareLinks.Commands.CreateShareLink;

public record CreateShareLinkCommand(
    Guid GalleryId,
    Guid UserId,
    string? ClientEmail,
    DownloadPermission DownloadPermission,
    DateTime? ExpiresAt) : IRequest<ShareLinkDto>;
