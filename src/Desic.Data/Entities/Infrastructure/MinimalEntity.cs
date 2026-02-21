namespace Desic.Data.Entities.Infrastructure;

public abstract class MinimalEntity : IReadOnlyMinimalEntity
{
    public Guid Id { get; set; }

    protected abstract Enums.EntityType EnumEntityType { get; }
    public virtual IReadOnlyEntityType GetEntityType() => EntityTypes.Get(EnumEntityType);
}
