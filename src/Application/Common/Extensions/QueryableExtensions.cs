using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Extensions;

internal static class QueryableExtensions
{
    internal const int DefaultTakeCount = 100;
    public static async Task<ListResult<T>> ToListResultAsync<T>(this IQueryable<T> source, int? startIndex = 0, int? takeCount = DefaultTakeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        return await source.ToListResultAsync<T, ListResult<T>>(startIndex: startIndex, takeCount: takeCount, includeTotalCount: includeTotalCount, cancellationToken: cancellationToken);
    }

    public static async Task<TR> ToListResultAsync<T, TR>(this IQueryable<T> source, int? startIndex = 0, int? takeCount = DefaultTakeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default) where TR : IListResult<T>, new()
    {
        startIndex ??= 0;
        takeCount ??= DefaultTakeCount;
        int? totalCount = includeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = await source.Skip(startIndex.Value).Take(takeCount.Value).ToListAsync(cancellationToken);
        return new TR()
        {
            Items = items,
            StartIndex = startIndex.Value,
            TotalCount = totalCount,
        };
    }
}
