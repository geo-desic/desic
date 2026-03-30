using Desic.Application.Common.Interfaces;

namespace Desic.Api.Tests.Unit;

public static class PaginationExtensions
{
    public static bool IsEquivalentTo(this IPagination source, IPagination compare)
    {
        return source.StartIndex == compare.StartIndex && source.Count == compare.Count && source.IncludeTotalCount == compare.IncludeTotalCount;
    }
}
