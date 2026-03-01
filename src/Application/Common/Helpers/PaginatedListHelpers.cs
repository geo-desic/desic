using Desic.Application.Common.Models;

namespace Desic.Application.Common.Helpers;

internal static class PaginatedListHelpers
{
    internal const int DefaultTakeCount = 100;
    public static async Task<PaginatedList<T>> ToPaginatedList<T>(this IQueryable<T> source, int offset = 0, int takeCount = DefaultTakeCount, bool includeTotalCount = false, CancellationToken cancellationToken = default)
    {
        return await PaginatedList<T>.CreateAsync(source: source, offset: offset, takeCount: takeCount, includeTotalCount: includeTotalCount, cancellationToken: cancellationToken);
    }
}
