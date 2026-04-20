using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Infrastructure.Persistence.Repositories;

public class GalleryRepository(MimreDbContext db) : IGalleryRepository
{
    public Task<Gallery?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.Galleries
            .Include(g => g.Albums)
            .FirstOrDefaultAsync(g => g.Id == id, ct);

    public Task<Gallery?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct = default) =>
        db.Galleries
            .Include(g => g.Albums)
            .FirstOrDefaultAsync(g => g.UserId == userId && g.Slug == slug, ct);

    public async Task<IReadOnlyList<Gallery>> GetByUserIdAsync(Guid userId, CancellationToken ct = default) =>
        await db.Galleries
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedOn)
            .ToListAsync(ct);

    public Task<bool> SlugExistsAsync(Guid userId, string slug, CancellationToken ct = default) =>
        db.Galleries.AnyAsync(g => g.UserId == userId && g.Slug == slug, ct);

    public void Add(Gallery gallery) => db.Galleries.Add(gallery);

    public void Remove(Gallery gallery) => db.Galleries.Remove(gallery);
}
