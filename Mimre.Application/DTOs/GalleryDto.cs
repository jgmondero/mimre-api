namespace Mimre.Application.DTOs;

public record GalleryDto(
    Guid Id,
    string Title,
    string Slug,
    bool IsPublished,
    Guid? CoverPhotoId,
    DateTime? ExpiresAt,
    bool HasPassword,
    DateTime CreatedOn,
    DateTime UpdatedOn);
