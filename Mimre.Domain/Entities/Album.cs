using Mimre.Domain.Common;

namespace Mimre.Domain.Entities;

public class Album : AuditableEntity
{
    public Guid GalleryId { get; private set; }
    public string Title { get; private set; } = default!;
    public int SortOrder { get; private set; }

    public Gallery Gallery { get; private set; } = default!;
    public ICollection<Photo> Photos { get; private set; } = [];

    private Album() { }

    public static Album Create(Guid galleryId, string title, int sortOrder = 0) =>
        new() { GalleryId = galleryId, Title = title, SortOrder = sortOrder };

    public void Rename(string title) => Title = title;
    public void Reorder(int sortOrder) => SortOrder = sortOrder;
}
