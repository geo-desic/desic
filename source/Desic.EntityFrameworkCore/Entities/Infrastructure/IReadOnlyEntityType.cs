namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

public interface IReadOnlyEntityType
{
    Guid Id { get; }
    string Name { get; }
}
