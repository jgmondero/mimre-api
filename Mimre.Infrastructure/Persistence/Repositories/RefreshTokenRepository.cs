using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Entities;
using Mimre.Domain.Interfaces.Repositories;

namespace Mimre.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(MimreDbContext db) : IRefreshTokenRepository
{
    public Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default) =>
        db.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Token == token, ct);

    public void Add(RefreshToken refreshToken) => db.RefreshTokens.Add(refreshToken);

    public void Remove(RefreshToken refreshToken) => db.RefreshTokens.Remove(refreshToken);
}
