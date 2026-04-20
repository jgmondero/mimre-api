using Mimre.Domain.Common;
using Mimre.Domain.Enums;

namespace Mimre.Domain.Entities;

public class ShareLink : AuditableEntity
{
    public Guid GalleryId { get; private set; }
    public string Token { get; private set; } = default!;
    public string? ClientEmail { get; private set; }
    public DownloadPermission DownloadPermission { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public int ViewCount { get; private set; }

    public Gallery Gallery { get; private set; } = default!;
    public ICollection<DownloadLog> DownloadLogs { get; private set; } = [];

    private ShareLink() { }

    public static ShareLink Create(
        Guid galleryId,
        string? clientEmail,
        DownloadPermission permission,
        DateTime? expiresAt) =>
        new()
        {
            GalleryId = galleryId,
            Token = Guid.NewGuid().ToString("N"), // e.g. "a3f9b1..."
            ClientEmail = clientEmail,
            DownloadPermission = permission,
            ExpiresAt = expiresAt
        };

    public bool IsExpired() => ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow;

    public void IncrementView() => ViewCount++;
}
