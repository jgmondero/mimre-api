namespace Mimre.Application.Common.Constants;

public static class CacheKeys
{
    // Gallery by share link token — heavily cached, clients hit this constantly
    public static string GalleryByToken(string token) =>
        $"gallery:token:{token}";

    // Photo list by album — cached per page/cursor
    public static string PhotosByAlbum(Guid albumId, Guid? cursor, int pageSize) =>
        $"photos:album:{albumId}:cursor:{cursor}:size:{pageSize}";

    // Gallery list by user — invalidated on any gallery change
    public static string GalleriesByUser(Guid userId, int page, int pageSize) =>
        $"galleries:user:{userId}:page:{page}:size:{pageSize}";

    // Album list by gallery
    public static string AlbumsByGallery(Guid galleryId, int page, int pageSize) =>
        $"albums:gallery:{galleryId}:page:{page}:size:{pageSize}";

    // Prefixes for bulk invalidation
    public static string UserGalleriesPrefix(Guid userId) =>
        $"galleries:user:{userId}";

    public static string GalleryAlbumsPrefix(Guid galleryId) =>
        $"albums:gallery:{galleryId}";

    public static string AlbumPhotosPrefix(Guid albumId) =>
        $"photos:album:{albumId}";
}
