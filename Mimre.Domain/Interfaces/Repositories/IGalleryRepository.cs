using Mimre.Domain.Entities;

namespace Mimre.Domain.Interfaces.Repositories;

public interface IGalleryRepository
{
    Task<Gallery?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Gallery?> GetBySlugAsync(Guid userId, string slug, CancellationToken ct = default);
    Task<IReadOnlyList<Gallery>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(Guid userId, string slug, CancellationToken ct = default);
    void Add(Gallery gallery);
    void Remove(Gallery gallery);
}
