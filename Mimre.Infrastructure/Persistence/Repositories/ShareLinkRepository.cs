using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Infrastructure.Persistence.Repositories;

public class ShareLinkRepository(MimreDbContext db) : IShareLinkRepository
{
    public Task<ShareLink?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        db.ShareLinks
            .Include(s => s.Gallery)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<ShareLink?> GetByTokenAsync(string token, CancellationToken ct = default) =>
        db.ShareLinks
            .Include(s => s.Gallery)
            .FirstOrDefaultAsync(s => s.Token == token, ct);

    public async Task<IReadOnlyList<ShareLink>> GetByGalleryIdAsync(Guid galleryId, CancellationToken ct = default) =>
        await db.ShareLinks
            .Where(s => s.GalleryId == galleryId)
            .OrderByDescending(s => s.CreatedOn)
            .ToListAsync(ct);

    public void Add(ShareLink shareLink) => db.ShareLinks.Add(shareLink);

    public void Remove(ShareLink shareLink) => db.ShareLinks.Remove(shareLink);
}
