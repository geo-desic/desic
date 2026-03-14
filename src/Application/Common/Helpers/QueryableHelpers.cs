using Desic.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Helpers;

internal static class QueryableHelpers
{
    internal const int DefaultTakeCount = 100;
    public static async Task<PaginatedList<T>> ToPaginatedList<T>(this IQueryable<T> source, int startIndex = 0, int? takeCount = DefaultTakeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        takeCount ??= DefaultTakeCount;
        int? totalCount = includeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = await source.Skip(startIndex).Take(takeCount.Value).ToListAsync(cancellationToken);
        return new PaginatedList<T>(items, startIndex, totalCount: totalCount);
    }
}
