namespace Desic.Domain.Shared.Entities;

public interface IReadOnlyEntityType
{
    Guid Id { get; }
    string Name { get; }
}
