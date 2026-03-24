using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Extensions;

internal static class QueryableExtensions
{
    public static async Task<ListResult<T>> ToListResultAsync<T>(this IQueryable<T> source, int startIndex = 0, int takeCount = ListRequests.DefaultCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        return await source.ToListResultAsync<T, ListResult<T>>(startIndex: startIndex, takeCount: takeCount, includeTotalCount: includeTotalCount, cancellationToken: cancellationToken);
    }

    public static async Task<TR> ToListResultAsync<T, TR>(this IQueryable<T> source, int startIndex = 0, int takeCount = ListRequests.DefaultCount, bool includeTotalCount = false, CancellationToken cancellationToken = default) where TR : IListResult<T>, new()
    {
        int? totalCount = includeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = new List<T>();
        if (takeCount > 0 && (!totalCount.HasValue || startIndex < totalCount)) // otherwise no items will be returned so no reason to perform query
        {
            var query = startIndex > 0 ? source.Skip(startIndex) : source;
            items = await query.Take(takeCount).ToListAsync(cancellationToken);
        }
        return new TR()
        {
            Items = items,
            StartIndex = startIndex,
            TotalCount = totalCount,
        };
    }
}
