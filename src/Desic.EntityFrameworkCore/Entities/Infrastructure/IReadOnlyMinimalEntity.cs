namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

public interface IReadOnlyMinimalEntity
{
    Guid Id { get; }
    IReadOnlyEntityType GetEntityType();
}
