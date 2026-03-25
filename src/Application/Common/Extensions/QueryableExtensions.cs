using Desic.Application.Common.Interfaces;
using Desic.Application.Common.Models;
using Microsoft.EntityFrameworkCore;

namespace Desic.Application.Common.Extensions;

internal static class QueryableExtensions
{
    public static async Task<ListResult<T>> ToListResultAsync<T>(this IQueryable<T> source, IPagination pagination, CancellationToken cancellationToken = default)
    {
        return await source.ToListResultAsync<T, ListResult<T>>(pagination: pagination, cancellationToken: cancellationToken);
    }

    public static async Task<TR> ToListResultAsync<T, TR>(this IQueryable<T> source, IPagination pagination, CancellationToken cancellationToken = default) where TR : IListResult<T>, new()
    {
        int? totalCount = pagination.IncludeTotalCount ? await source.CountAsync(cancellationToken) : null;
        var items = new List<T>();
        if (pagination.Count > 0 && (!totalCount.HasValue || pagination.StartIndex < totalCount)) // otherwise no items will be returned so no reason to perform query
        {
            var query = pagination.StartIndex > 0 ? source.Skip(pagination.StartIndex) : source;
            items = await query.Take(pagination.Count).ToListAsync(cancellationToken);
        }
        return new TR()
        {
            Items = items,
            StartIndex = pagination.StartIndex,
            TotalCount = totalCount,
        };
    }
}
