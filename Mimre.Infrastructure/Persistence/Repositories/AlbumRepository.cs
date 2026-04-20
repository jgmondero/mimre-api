using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Infrastructure.Persistence.Repositories;

public class AlbumRepository(MimreDbContext db) : IAlbumRepository
{
    public Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Albums
            .Include(a => a.Photos)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public async Task<IReadOnlyList<Album>> GetByGalleryIdAsync(Guid galleryId, CancellationToken ct = default) =>
        await db.Albums
            .Where(a => a.GalleryId == galleryId)
            .OrderBy(a => a.SortOrder)
            .ToListAsync(ct);

    public void Add(Album album) => db.Albums.Add(album);

    public void Remove(Album album) => db.Albums.Remove(album);
}
