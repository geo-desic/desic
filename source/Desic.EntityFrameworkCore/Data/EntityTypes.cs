using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Entities.Infrastructure;

namespace Desic.EntityFrameworkCore.Data;

internal static class EntityTypes
{
    internal static IList<EntityType> Generate()
    {
        return [.. _entityTypes.Select(x => new EntityType { Id = x.Value.Id, Name = x.Value.Name })];
    }

    internal static ReadOnlyEntityType Get(Enums.EntityType entityType)
    {
        return _entityTypes[entityType];
    }

    private static SortedList<Enums.EntityType, ReadOnlyEntityType> GenerateEntityTypesFromEnum()
    {
        var result = new SortedList<Enums.EntityType, ReadOnlyEntityType>();
        foreach (var value in Enum.GetValues<Enums.EntityType>())
        {
            var integerIdString = $"{(int)value}";
            var guidString = "00000000"[..^integerIdString.Length] + integerIdString + "-0000-0000-0000-000000000000";
            result.Add(value, new ReadOnlyEntityType { Id = new(guidString), Name = Enum.GetName(value)! });
        }
        return result;
    }

    private static readonly SortedList<Enums.EntityType, ReadOnlyEntityType> _entityTypes = GenerateEntityTypesFromEnum();
}
