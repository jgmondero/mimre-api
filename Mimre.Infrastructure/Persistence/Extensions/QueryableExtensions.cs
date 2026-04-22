using Microsoft.EntityFrameworkCore;
using Mimre.Domain.Common;

namespace Mimre.Infrastructure.Persistence.Extensions;

public static class QueryableExtensions
{
    // Offset-based pagination for admin lists
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        int page,
        int pageSize,
        CancellationToken ct = default)
    {
        // Clamp values
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, PagedResult<T>.MaxPageSize);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount
        };
    }

    // Cursor-based pagination for infinite scroll
    public static async Task<CursorResult<T>> ToCursorResultAsync<T>(
        this IQueryable<T> query,
        int pageSize,
        CancellationToken ct = default)
        where T : class
    {
        // Clamp page size
        pageSize = Math.Clamp(pageSize, 1, CursorResult<T>.MaxPageSize);

        // Fetch one extra item to determine if there's a next page
        var items = await query
            .Take(pageSize + 1)
            .ToListAsync(ct);

        var hasNextPage = items.Count > pageSize;

        if (hasNextPage)
            items.RemoveAt(items.Count - 1); // remove the extra item

        return new CursorResult<T>
        {
            Items = items,
            PageSize = pageSize,
            NextCursor = null // set by the repository after getting the ID
        };
    }
}
