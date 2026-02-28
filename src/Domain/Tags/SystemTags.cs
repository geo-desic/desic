namespace Desic.Domain.Tags;

public static class SystemTags
{
    public static IList<Tag> Generate()
    {
        var tagSystem = Get(SystemTag.System);
        return [.. _systemTags.Select(x => new Tag
        {
            Id = x.Value.Id,
            CreatedByTypeId = Tag.ClassEntityType.Id,
            CreatedById = tagSystem.Id,
            ModifiedByTypeId = Tag.ClassEntityType.Id,
            ModifiedById = tagSystem.Id,
            Name = x.Value.Name,
        })];
    }

    public static ReadOnlyTag Get(SystemTag systemTag)
    {
        return _systemTags[systemTag];
    }

    private static readonly SortedList<SystemTag, ReadOnlyTag> _systemTags = GenerateSystemTagsFromEnum();

    private static SortedList<SystemTag, ReadOnlyTag> GenerateSystemTagsFromEnum()
    {
        var result = new SortedList<SystemTag, ReadOnlyTag>();
        var entityTypeTagIdString = Tag.ClassEntityType.Id.ToString();
        foreach (var value in Enum.GetValues<SystemTag>())
        {
            var integerIdString = $"{(int)value}";
            var guidString = entityTypeTagIdString[..^integerIdString.Length] + integerIdString;
            result.Add(value, new ReadOnlyTag { Id = new(guidString), Name = Enum.GetName(value)! });
        }
        return result;
    }
}
