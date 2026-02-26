namespace Desic.Domain.Common.Entities;

public interface IStaticEntityType
{
    static abstract IReadOnlyEntityType EntityType { get; }
}
