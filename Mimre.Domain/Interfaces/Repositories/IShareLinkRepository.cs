using Mimre.Domain.Common;
using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Repositories;

public interface IShareLinkRepository
{
    Task<ShareLink?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ShareLink?> GetByTokenAsync(string token, CancellationToken ct = default);
    Task<PagedResult<ShareLink>> GetByGalleryIdAsync(Guid galleryId, int page, int pageSize, CancellationToken ct = default);
    void Add(ShareLink shareLink);
    void Remove(ShareLink shareLink);
}
