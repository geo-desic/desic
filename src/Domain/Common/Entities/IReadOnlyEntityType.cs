namespace Desic.Domain.Common.Entities;

public interface IReadOnlyEntityType
{
    Guid Id { get; }
    string Name { get; }
}
