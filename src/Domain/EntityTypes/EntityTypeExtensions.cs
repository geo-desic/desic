using Desic.Shared.Extensions;

namespace Desic.Domain.EntityTypes;

public static class EntityTypeExtensions
{
    public static bool IsEquivalentTo(this IReadOnlyEntityTypeReferenceData? item1, IReadOnlyEntityTypeReferenceData? item2)
    {
        if (item1 == null || item2 == null) return item1.NullablyEquivalentTo(item2);
        return item1.Key == item2.Key && item1.Name == item2.Name;
    }
}
