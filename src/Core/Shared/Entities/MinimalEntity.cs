using Desic.Core.EntityTypes;

namespace Desic.Core.Shared.Entities;

public abstract class MinimalEntity : IReadOnlyMinimalEntity
{
    public Guid Id { get; set; }

    protected abstract SystemEntityType EnumEntityType { get; }
    public virtual IReadOnlyEntityType GetEntityType() => SystemEntityTypes.Get(EnumEntityType);
}
