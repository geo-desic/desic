using Desic.Data.EntityTypes;

namespace Desic.Data.Shared.Entities;

public abstract class MinimalEntity : IReadOnlyMinimalEntity
{
    public Guid Id { get; set; }

    protected abstract SystemEntityType EnumEntityType { get; }
    public virtual IReadOnlyEntityType GetEntityType() => EntityTypes.SystemEntityTypes.Get(EnumEntityType);
}
