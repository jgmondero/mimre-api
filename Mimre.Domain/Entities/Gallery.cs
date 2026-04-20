using Mimre.Domain.Common;
using Mimre.Domain.Events;

namespace Mimre.Domain.Entities;

public class Gallery : AuditableEntity
{
    public Guid UserId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public Guid? CoverPhotoId { get; private set; }
    public bool IsPublished { get; private set; }
    public DateTime? ExpiresAt { get; private set; }
    public string? PasswordHash { get; private set; }

    public User User { get; private set; } = default!;
    public ICollection<Album> Albums { get; private set; } = [];
    public ICollection<ShareLink> ShareLinks { get; private set; } = [];

    private Gallery() { }

    public static Gallery Create(Guid userId, string title, string slug) =>
        new()
        {
            UserId = userId,
            Title = title,
            Slug = slug.ToLowerInvariant()
        };

    public void Publish()
    {
        IsPublished = true;
        RaiseDomainEvent(new GalleryPublishedEvent(Id));
    }

    public void Unpublish() => IsPublished = false;

    public void SetPassword(string? passwordHash) => PasswordHash = passwordHash;

    public void SetExpiry(DateTime? expiresAt) => ExpiresAt = expiresAt;

    public void SetCoverPhoto(Guid? photoId) => CoverPhotoId = photoId;

    public void UpdateTitle(string title) => Title = title;

    public void UpdateSlug(string slug) => Slug = slug.ToLowerInvariant();
}
