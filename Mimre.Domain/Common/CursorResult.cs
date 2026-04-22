namespace Mimre.Domain.Common;

public record CursorResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];
    public Guid? NextCursor { get; init; }
    public bool HasNextPage => NextCursor.HasValue;
    public int PageSize { get; init; }

    public const int MaxPageSize = 100;
    public const int DefaultPageSize = 30;
}
