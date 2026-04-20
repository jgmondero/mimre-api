using Mimre.Application.Common.Interfaces;
using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Infrastructure.Persistence;

public class UnitOfWork(
    MimreDbContext db,
    IUserRepository users,
    IGalleryRepository galleries,
    IAlbumRepository albums,
    IPhotoRepository photos,
    IShareLinkRepository shareLinks) : IUnitOfWork
{
    public IUserRepository Users => users;
    public IGalleryRepository Galleries => galleries;
    public IAlbumRepository Albums => albums;
    public IPhotoRepository Photos => photos;
    public IShareLinkRepository ShareLinks => shareLinks;

    public Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        db.SaveChangesAsync(ct);

    public IEnumerable<object> GetTrackedEntities() =>
        db.ChangeTracker.Entries().Select(e => e.Entity);
}
