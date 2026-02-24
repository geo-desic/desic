namespace Desic.Core.Tags;

public interface IReadOnlyMinimalTag
{
    Guid Id { get; }
    string Name { get; }
}
