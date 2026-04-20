using Mimre.Domain.Enums;

namespace Mimre.Application.DTOs;

public record ShareLinkDto(
    Guid Id,
    Guid GalleryId,
    string Token,
    string? ClientEmail,
    DownloadPermission DownloadPermission,
    DateTime? ExpiresAt,
    int ViewCount,
    string ShareUrl);   // e.g. https://mimre.app/c/{token}
