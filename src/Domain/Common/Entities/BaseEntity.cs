using Desic.Domain.EntityTypes;

namespace Desic.Domain.Common.Entities;

public abstract class BaseEntity : IReadOnlyMinimalEntity
{
    public abstract SystemEntityType SystemEntityType { get; }

    public Guid Id { get; set; }
}
