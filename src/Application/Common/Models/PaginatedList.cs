using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Models;

public class PaginatedList<T>(IReadOnlyCollection<T> items, int offset, int? totalCount = null)
{
    public int StartIndex { get; } = offset;
    public int? TotalCount { get; } = totalCount;
    public IReadOnlyCollection<T> Items { get; } = items;
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int startIndex, int takeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        int? totalCount = includeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = await source.Skip(startIndex).Take(takeCount).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, startIndex, totalCount: totalCount);
    }
}
