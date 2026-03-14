using Desic.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Helpers;

internal static class QueryableHelpers
{
    internal const int DefaultTakeCount = 100;
    public static async Task<ListResult<T>> ToListResultAsync<T>(this IQueryable<T> source, int? startIndex = 0, int? takeCount = DefaultTakeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        startIndex ??= 0;
        takeCount ??= DefaultTakeCount;
        int? totalCount = includeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = await source.Skip(startIndex.Value).Take(takeCount.Value).ToListAsync(cancellationToken);
        return new ListResult<T>(items: items, startIndex: startIndex.Value, totalCount: totalCount);
    }
}
