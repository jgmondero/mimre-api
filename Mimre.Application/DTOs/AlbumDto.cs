namespace Mimre.Application.DTOs;

public record AlbumDto(
    Guid Id,
    Guid GalleryId,
    string Title,
    int SortOrder,
    int PhotoCount);
