using Mimre.Domain.Common;
using Mimre.Domain.Enums;

namespace Mimre.Domain.Entities;

public class User : AuditableEntity
{
    public string Email { get; private set; } = default!;
    public string PasswordHash { get; private set; } = default!;
    public string FullName { get; private set; } = default!;
    public PlanTier PlanTier { get; private set; } = PlanTier.Free;
    public long StorageUsedBytes { get; private set; }

    public ICollection<Gallery> Galleries { get; private set; } = [];

    private User() { }

    public static User Create(string email, string passwordHash, string fullName) =>
        new()
        {
            Email = email.ToLowerInvariant(),
            PasswordHash = passwordHash,
            FullName = fullName
        };

    public void AddStorageUsage(long bytes)
    {
        if (StorageUsedBytes + bytes < 0)
            throw new InvalidOperationException("Storage usage cannot be negative.");
        StorageUsedBytes += bytes;
    }

    public void UpdateProfile(string fullName) => FullName = fullName;
    public void SetPlan(PlanTier tier) => PlanTier = tier;
}
