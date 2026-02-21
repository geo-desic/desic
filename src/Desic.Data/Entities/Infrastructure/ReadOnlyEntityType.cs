namespace Desic.Data.Entities.Infrastructure;

public class ReadOnlyEntityType : IReadOnlyEntityType, IReadOnlyMinimalEntity
{
    public Guid Id { get; init; }
    public required string Name { get; init; }
    public IReadOnlyEntityType GetEntityType() => this;
}
