using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Common;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;
using Mimre.Infrastructure.Persistence.Extensions;

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

    public Task<PagedResult<ShareLink>> GetByGalleryIdAsync(Guid galleryId, int page, int pageSize, CancellationToken ct = default) =>
        db.ShareLinks
            .Where(s => s.GalleryId == galleryId)
            .OrderByDescending(s => s.CreatedOn)
            .ToPagedResultAsync(page, pageSize, ct);

    public void Add(ShareLink shareLink) => db.ShareLinks.Add(shareLink);

    public void Remove(ShareLink shareLink) => db.ShareLinks.Remove(shareLink);
}
