using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Entities;

public abstract class BaseEntity : IReadOnlyMinimalEntity
{
    public abstract IReadOnlyEntityType GetEntityType();

    public Guid Id { get; set; }
}
