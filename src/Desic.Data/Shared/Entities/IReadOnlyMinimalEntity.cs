namespace Desic.Data.Shared.Entities;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }
    IReadOnlyEntityType GetEntityType();
}
