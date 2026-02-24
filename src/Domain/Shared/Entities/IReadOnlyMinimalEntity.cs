namespace Desic.Domain.Shared.Entities;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }
    IReadOnlyEntityType GetEntityType();
}
