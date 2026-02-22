namespace Desic.Data.Tags;

public interface IReadOnlyMinimalTag
{
    Guid Id { get; }
    string Name { get; }
}
