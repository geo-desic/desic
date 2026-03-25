namespace Desic.Application.Common.Infrastructure;

internal static class Filters
{
    public const string DescriptionNonNullStringContains = "If non-null a filter will be applied to the query for items with this property whose value contains the supplied value as a substring (not necessarily properly)";
    public const string DescriptionNonNullExact = "If non-null a filter will be applied to the query for items with this property whose value matches the supplied value exactly";
}
