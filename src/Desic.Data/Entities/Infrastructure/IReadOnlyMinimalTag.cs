namespace Desic.Data.Entities.Infrastructure;

public interface IReadOnlyMinimalTag
{
    Guid Id { get; }
    string Name { get; }
}
