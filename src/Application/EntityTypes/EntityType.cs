using Desic.Domain.EntityTypes;
using System.Diagnostics.CodeAnalysis;

namespace Desic.Application.EntityTypes;

public class EntityType : IReadOnlyEntityType
{
    public EntityType() { }

    [SetsRequiredMembers]
    public EntityType(IReadOnlyEntityType source) : this()
    {
        Key = source.Key;
        Name = source.Name;
    }

    public required string Key { get; set; }
    public required string Name { get; set; }
}
