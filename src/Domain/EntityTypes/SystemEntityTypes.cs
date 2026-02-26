namespace Desic.Domain.EntityTypes;

public static class SystemEntityTypes
{
    public static IList<EntityType> Generate()
    {
        return [.. _entityTypes.Select(x => new EntityType { Id = x.Value.Id, Name = x.Value.Name })];
    }

    public static ReadOnlyEntityType Get(SystemEntityType entityType)
    {
        return _entityTypes[entityType];
    }

    private static SortedList<SystemEntityType, ReadOnlyEntityType> GenerateEntityTypesFromEnum()
    {
        var result = new SortedList<SystemEntityType, ReadOnlyEntityType>();
        foreach (var value in Enum.GetValues<SystemEntityType>())
        {
            var integerIdString = $"{(int)value}";
            var guidString = "00000000"[..^integerIdString.Length] + integerIdString + "-0000-0000-0000-000000000000";
            result.Add(value, new ReadOnlyEntityType { Id = new(guidString), Name = Enum.GetName(value)! });
        }
        return result;
    }

    private static readonly SortedList<SystemEntityType, ReadOnlyEntityType> _entityTypes = GenerateEntityTypesFromEnum();
}
