namespace Desic.Domain.Tags;

public static class SystemTags
{
    public static SystemTag? GetById(Guid Id) => _dictionaryById.TryGetValue(Id, out var value) ? value : null;
    public static SystemTag? GetByName(string name) => _dictionaryByName.TryGetValue(name, out var value) ? value : null;

    public static IEnumerable<Tag> AllAsEntities()
    {
        foreach (var value in All()) yield return value.ToEntity();
    }

    // when adding a new system tag make sure to also add it to the All() method
    // all fields (Id, Name) should be unique (case-insensitive) across all records
    // do not change any values for existing records after it has been added to a non-development database
    // this list should be ordered by Id
    #pragma warning disable format
    public static readonly SystemTag System               = new(Id: new("00000003-0000-0000-0000-000000000001"), Name: nameof(System));
    #pragma warning restore format

    internal static IEnumerable<SystemTag> All()
    {
        yield return System;
    }

    private static readonly Dictionary<Guid, SystemTag> _dictionaryById = All().ToDictionary(x => x.Id);
    private static readonly Dictionary<string, SystemTag> _dictionaryByName = All().ToDictionary(x => x.Name, comparer: StringComparer.OrdinalIgnoreCase);
}
