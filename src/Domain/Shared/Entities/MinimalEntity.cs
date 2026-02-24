using Desic.Domain.EntityTypes;

namespace Desic.Domain.Shared.Entities;

public abstract class MinimalEntity : IReadOnlyMinimalEntity
{
    public Guid Id { get; set; }

    protected abstract SystemEntityType EnumEntityType { get; }
    public virtual IReadOnlyEntityType GetEntityType() => SystemEntityTypes.Get(EnumEntityType);
}
