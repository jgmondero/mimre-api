using Mimre.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
    void Add(RefreshToken refreshToken);
    void Remove(RefreshToken refreshToken);
}
