namespace Desic.Core.Shared.Entities;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }
    IReadOnlyEntityType GetEntityType();
}
