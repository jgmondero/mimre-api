using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Common;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;
using Mimre.Infrastructure.Persistence.Extensions;

namespace Mimre.Infrastructure.Persistence.Repositories;

public class AlbumRepository(MimreDbContext db) : IAlbumRepository
{
    public Task<Album?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Albums
            .Include(a => a.Photos)
            .FirstOrDefaultAsync(a => a.Id == id, ct);

    public Task<PagedResult<Album>> GetByGalleryIdAsync(Guid galleryId, int page, int pageSize, CancellationToken ct = default) =>
        db.Albums
            .Where(a => a.GalleryId == galleryId)
            .OrderBy(a => a.SortOrder)
            .ToPagedResultAsync(page, pageSize, ct);

    public void Add(Album album) => db.Albums.Add(album);

    public void Remove(Album album) => db.Albums.Remove(album);
}
