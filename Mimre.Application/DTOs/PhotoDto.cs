using Mimre.Domain.Enums;

namespace Mimre.Application.DTOs;

public record PhotoDto(
    Guid Id,
    Guid AlbumId,
    string OriginalFileName,
    string ThumbnailUrl,        // public CDN URL
    string? WatermarkedUrl,     // public CDN URL
    long FileSizeBytes,
    int Width,
    int Height,
    int SortOrder,
    PhotoStatus Status);
