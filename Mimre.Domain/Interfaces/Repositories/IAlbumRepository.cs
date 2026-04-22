using Mimre.Domain.Common;
using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Repositories;

public interface IAlbumRepository
{
    Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<PagedResult<Album>> GetByGalleryIdAsync(Guid galleryId, int page, int pageSize, CancellationToken ct = default);
    void Add(Album album);
    void Remove(Album album);
}
