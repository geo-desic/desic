namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

internal interface IReadOnlyMinimalTag
{
    Guid Id { get; }
    string Name { get; }
}
