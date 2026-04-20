using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Repositories;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Photo>> GetByAlbumIdAsync(Guid albumId, CancellationToken ct = default);
    void Add(Photo photo);
    void Remove(Photo photo);
}
