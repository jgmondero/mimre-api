using Mimre.Domain.Common;

namespace Mimre.Domain.Entities;

public class DownloadLog : BaseEntity
{
    public Guid ShareLinkId { get; private set; }
    public Guid? PhotoId { get; private set; } // null = full gallery zip
    public string IpAddress { get; private set; } = default!;
    public DateTime DownloadedAt { get; private set; } = DateTime.UtcNow;

    public ShareLink ShareLink { get; private set; } = default!;

    private DownloadLog() { }

    public static DownloadLog Create(Guid shareLinkId, Guid? photoId, string ipAddress) =>
        new() { ShareLinkId = shareLinkId, PhotoId = photoId, IpAddress = ipAddress };
}
