using Desic.Domain.EntityTypes;
using System.Diagnostics.CodeAnalysis;

namespace Desic.Application.EntityTypes;

public class EntityType : IReadOnlyEntityTypeReferenceData
{
    public EntityType() { }

    [SetsRequiredMembers]
    public EntityType(IReadOnlyEntityTypeReferenceData source) : this()
    {
        Key = source.Key;
        Name = source.Name;
    }

    public required string Key { get; set; }
    public required string Name { get; set; }

    bool IEquatable<IReadOnlyEntityTypeReferenceData>.Equals(IReadOnlyEntityTypeReferenceData? compare) => this.IsEquivalentTo(compare);
}
