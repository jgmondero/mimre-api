using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Infrastructure.Persistence.Repositories;

public class PhotoRepository(MimreDbContext db) : IPhotoRepository
{
    public Task<Photo?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
    db.Photos
        .Include(p => p.Album)
        .ThenInclude(a => a.Gallery)
        .FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<IReadOnlyList<Photo>> GetByAlbumIdAsync(Guid albumId, CancellationToken ct = default) =>
        await db.Photos
            .Where(p => p.AlbumId == albumId)
            .OrderBy(p => p.SortOrder)
            .ToListAsync(ct);

    public void Add(Photo photo) => db.Photos.Add(photo);

    public void Remove(Photo photo) => db.Photos.Remove(photo);
}
