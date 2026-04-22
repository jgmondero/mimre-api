using Mimre.Domain.Common;
using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Repositories;

public interface IPhotoRepository
{
    Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<CursorResult<Photo>> GetByAlbumIdAsync(Guid albumId, Guid? cursor, int pageSize, CancellationToken ct = default);
    void Add(Photo photo);
    void Remove(Photo photo);
}
