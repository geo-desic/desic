using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Models;

public class PaginatedList<T>(IReadOnlyCollection<T> items, int offset, int? totalCount = null)
{
    public int Offset { get; } = offset;
    public int? TotalCount { get; } = totalCount;
    public IReadOnlyCollection<T> Items { get; } = items;
    public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int offset, int takeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        int? totalCount = includeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = await source.Skip(offset).Take(takeCount).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, offset, totalCount: totalCount);
    }
}
