using Mimre.Domain.Common;

namespace Mimre.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = default!;
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; }
    public DateTime CreatedOn { get; private set; }

    public User User { get; private set; } = default!;

    private RefreshToken() { }

    public static RefreshToken Create(Guid userId, string token, DateTime expiresAt) =>
        new()
        {
            UserId = userId,
            Token = token,
            ExpiresAt = expiresAt,
            CreatedOn = DateTime.UtcNow
        };

    public bool IsExpired() => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive() => !IsRevoked && !IsExpired();

    public void Revoke() => IsRevoked = true;
}
