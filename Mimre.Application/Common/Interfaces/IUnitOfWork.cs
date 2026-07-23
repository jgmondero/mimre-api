using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Application.Common.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    IGalleryRepository Galleries { get; }
    IAlbumRepository Albums { get; }
    IPhotoRepository Photos { get; }
    IShareLinkRepository ShareLinks { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
    IEnumerable<object> GetTrackedEntities();
}
