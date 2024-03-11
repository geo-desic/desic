using Desic.EntityFrameworkCore.Entities;

namespace Desic.EntityFrameworkCore.Data
{
    internal static class EntityTypes
    {
        static EntityTypes()
        {
            _entityTypes = GenerateEntityTypesFromEnum();
        }

        internal static IList<EntityType> Generate()
        {
            return _entityTypes.Select(x => new EntityType { Id = x.Value.Id, Name = x.Value.Name }).ToList();
        }

        internal static ReadOnlyEntityType Get(Enums.EntityType entityType)
        {
            return _entityTypes[entityType];
        }

        private static SortedList<Enums.EntityType, ReadOnlyEntityType> GenerateEntityTypesFromEnum()
        {
            var result = new SortedList<Enums.EntityType, ReadOnlyEntityType>();
            foreach (var value in (Enums.EntityType[])Enum.GetValues(typeof(Enums.EntityType)))
            {
                var integerIdString = $"{(int)value}";
                var guidString = "00000000"[..^integerIdString.Length] + integerIdString + "-0000-0000-0000-000000000000";
                result.Add(value, new ReadOnlyEntityType { Id = new(guidString), Name = Enum.GetName(value)! });
            }
            return result;
        }

        private static readonly SortedList<Enums.EntityType, ReadOnlyEntityType> _entityTypes;

        internal class ReadOnlyEntityType
        {
            public Guid Id { get; init; }
            public required string Name { get; init; }
        }
    }
}
