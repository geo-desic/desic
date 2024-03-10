using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Enums;

namespace Desic.EntityFrameworkCore.Data
{
    internal static class Tags
    {
        static Tags()
        {
            _entityTypeTag = EntityTypes.Get(Enums.EntityType.Tag)!;
            _systemTags = GenerateSystemTagsFromEnum();
        }

        internal static IList<Tag> Generate()
        {
            var tagSystem = Get(SystemTag.System)!;
            return _systemTags.Select(x => new Tag
            {
                Id = x.Value.Id,
                CreatedByTypeId = _entityTypeTag.Id,
                CreatedById = tagSystem.Id,
                ModifiedByTypeId = _entityTypeTag.Id,
                ModifiedById = tagSystem.Id,
                Name = x.Value.Name,
            }).ToList();
        }

        internal static ReadOnlyTag? Get(SystemTag systemTag)
        {
            return _systemTags.TryGetValue(systemTag, out var result) ? result : null;
        }

        private static readonly EntityTypes.ReadOnlyEntityType _entityTypeTag;
        private static readonly SortedList<SystemTag, ReadOnlyTag> _systemTags;

        private static SortedList<SystemTag, ReadOnlyTag> GenerateSystemTagsFromEnum()
        {
            var result = new SortedList<SystemTag, ReadOnlyTag>();
            var entityTypeTagIdString = _entityTypeTag.Id.ToString();
            foreach (var value in (SystemTag[])Enum.GetValues(typeof(SystemTag)))
            {
                var integerIdString = $"{(int)value}";
                var guidString = entityTypeTagIdString[..^integerIdString.Length] + integerIdString;
                result.Add(value, new ReadOnlyTag { Id = new(guidString), Name = Enum.GetName(value)! });
            }
            return result;
        }

        internal class ReadOnlyTag
        {
            public Guid Id { get; init; }
            public required string Name { get; init; }
        }
    }
}
