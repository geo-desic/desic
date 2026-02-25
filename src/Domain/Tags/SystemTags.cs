using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;

namespace Desic.Domain.Tags;

public static class SystemTags
{
    public static IList<Tag> Generate()
    {
        var tagSystem = Get(SystemTag.System);
        return [.. _systemTags.Select(x => new Tag
        {
            Id = x.Value.Id,
            CreatedByTypeId = _entityTypeTag.Id,
            CreatedById = tagSystem.Id,
            ModifiedByTypeId = _entityTypeTag.Id,
            ModifiedById = tagSystem.Id,
            Name = x.Value.Name,
        })];
    }

    public static ReadOnlyTag Get(SystemTag systemTag)
    {
        return _systemTags[systemTag];
    }

    private static readonly ReadOnlyEntityType _entityTypeTag = SystemEntityTypes.Get(SystemEntityType.Tag);
    private static readonly SortedList<SystemTag, ReadOnlyTag> _systemTags = GenerateSystemTagsFromEnum();

    private static SortedList<SystemTag, ReadOnlyTag> GenerateSystemTagsFromEnum()
    {
        var result = new SortedList<SystemTag, ReadOnlyTag>();
        var entityTypeTagIdString = _entityTypeTag.Id.ToString();
        foreach (var value in Enum.GetValues<SystemTag>())
        {
            var integerIdString = $"{(int)value}";
            var guidString = entityTypeTagIdString[..^integerIdString.Length] + integerIdString;
            result.Add(value, new ReadOnlyTag { Id = new(guidString), Name = Enum.GetName(value)! });
        }
        return result;
    }
}
