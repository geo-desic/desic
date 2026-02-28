namespace Desic.Domain.EntityTypes;

public static class SystemEntityTypes
{
    public static SystemEntityType? GetById(Guid Id) => _dictionaryById.TryGetValue(Id, out var value) ? value : null;
    public static SystemEntityType? GetByKey(string key) => _dictionaryByKey.TryGetValue(key, out var value) ? value : null;
    public static SystemEntityType? GetByName(string name) => _dictionaryByName.TryGetValue(name, out var value) ? value : null;

    public static IEnumerable<EntityType> AllAsEntities()
    {
        foreach (var value in All()) yield return new() { Id = value.Id, Key = value.Key, Name = value.Name };
    }

    // when adding a new entity type make sure to also add it to the All() method
    // all fields (Id, Name, and Key) should be unique (case-insensitive) across all records
    // all keys should be exactly 4 alphabetic ascii lowercase characters (i.e. each character has an ascii decimal value between 97 and 122 inclusive)
    // do not change any values for existing records after it has been added to a non-development database
    // this list should be ordered by Id
    #pragma warning disable format
    public static readonly SystemEntityType Unspecified          = new(Id: new("00000001-0000-0000-0000-000000000000"), Key: "unsp", Name: nameof(Unspecified));
    public static readonly SystemEntityType EntityType           = new(Id: new("00000002-0000-0000-0000-000000000000"), Key: "enty", Name: nameof(EntityType));
    public static readonly SystemEntityType Tag                  = new(Id: new("00000003-0000-0000-0000-000000000000"), Key: "tags", Name: nameof(Tag));
    public static readonly SystemEntityType User                 = new(Id: new("00000004-0000-0000-0000-000000000000"), Key: "user", Name: nameof(User));
    public static readonly SystemEntityType Iso3166Country       = new(Id: new("00000005-0000-0000-0000-000000000000"), Key: "ctry", Name: nameof(Iso3166Country));
    #pragma warning restore format

    // this should be ordered by Id (i.e. same order as list above)
    private static IEnumerable<SystemEntityType> All()
    {
        yield return Unspecified;
        yield return EntityType;
        yield return Tag;
        yield return User;
        yield return Iso3166Country;
    }

    private static readonly Dictionary<Guid, SystemEntityType> _dictionaryById = All().ToDictionary(x => x.Id);
    private static readonly Dictionary<string, SystemEntityType> _dictionaryByName = All().ToDictionary(x => x.Name, comparer: StringComparer.OrdinalIgnoreCase);
    private static readonly Dictionary<string, SystemEntityType> _dictionaryByKey = All().ToDictionary(x => x.Key, comparer: StringComparer.OrdinalIgnoreCase);
}
