namespace Desic.Data.Entities.Infrastructure;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }
    IReadOnlyEntityType GetEntityType();
}
