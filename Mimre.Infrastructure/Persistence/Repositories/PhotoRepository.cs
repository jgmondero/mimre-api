using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Common;
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

    public async Task<CursorResult<Photo>> GetByAlbumIdAsync(Guid albumId, Guid? cursor, int pageSize, CancellationToken ct = default)
    {
        pageSize = Math.Clamp(pageSize, 1, CursorResult<Photo>.MaxPageSize);

        var query = db.Photos
            .Where(p => p.AlbumId == albumId);

        // If cursor provided, only fetch photos after the cursor position
        if (cursor.HasValue)
        {
            var cursorPhoto = await db.Photos
                .FirstOrDefaultAsync(p => p.Id == cursor.Value, ct);

            if (cursorPhoto is not null)
                query = query.Where(p =>
                    p.SortOrder > cursorPhoto.SortOrder ||
                    (p.SortOrder == cursorPhoto.SortOrder && p.Id != cursorPhoto.Id));
        }

        // Fetch one extra to detect next page
        var items = await query
            .OrderBy(p => p.SortOrder)
            .ThenBy(p => p.Id)
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNextPage = items.Count > pageSize;

        if (hasNextPage)
            items.RemoveAt(items.Count - 1);

        return new CursorResult<Photo>
        {
            Items = items,
            PageSize = pageSize,
            NextCursor = hasNextPage ? items[^1].Id : null
        };
    }

    public void Add(Photo photo) => db.Photos.Add(photo);

    public void Remove(Photo photo) => db.Photos.Remove(photo);
}
