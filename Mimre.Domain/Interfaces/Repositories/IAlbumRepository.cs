using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Repositories;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Album>> GetByGalleryIdAsync(Guid galleryId, CancellationToken ct = default);
    void Add(Album album);
    void Remove(Album album);
}
