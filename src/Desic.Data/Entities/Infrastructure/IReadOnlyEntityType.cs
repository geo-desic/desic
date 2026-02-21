namespace Desic.Data.Entities.Infrastructure;

public interface IReadOnlyEntityType
{
    Guid Id { get; }
    string Name { get; }
}
