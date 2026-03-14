using Desic.Application.Common.Models;

namespace Desic.Application.Common.Helpers;

internal static class QueryableHelpers
{
    private const int DefaultTakeCount = 100;
    public static async Task<PaginatedList<T>> ToPaginatedList<T>(this IQueryable<T> source, int startIndex = 0, int? takeCount = DefaultTakeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        takeCount ??= DefaultTakeCount;
        return await PaginatedList<T>.CreateAsync(source: source, startIndex: startIndex, takeCount: takeCount.Value, includeTotalCount: includeTotalCount, cancellationToken: cancellationToken);
    }
}
