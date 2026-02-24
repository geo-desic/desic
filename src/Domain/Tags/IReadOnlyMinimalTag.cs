namespace Desic.Domain.Tags;

public interface IReadOnlyMinimalTag
{
    Guid Id { get; }
    string Name { get; }
}
