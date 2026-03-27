using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.EntityTypes;

public class EntityType : BaseEntity, IStaticEntityType, IReadOnlyNameable, IReadOnlyEntityType
{
    public static SystemEntityType ClassEntityType => SystemEntityTypes.EntityType;
    public override SystemEntityType SystemEntityType => ClassEntityType;

    public required string Key { get; set; }
    public required string Name { get; set; }

    public const int LengthKey = 4;
    public const int MaxLengthName = 50;
    public const int MinLengthName = 2;
    public const string RegexKey = "^[a-z]*$"; // lowercase alphabetic characters
    public const string RegexName = "^[A-Z]{1}[a-zA-Z0-9]*$"; // starts with an upper case alphabetic character and contains only alphanumeric characters

    bool IEquatable<IReadOnlyEntityType>.Equals(IReadOnlyEntityType? compare) => this.IsEquivalentTo(compare);
}
