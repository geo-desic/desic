namespace Desic.Domain.Common.Interfaces;

public interface IReadOnlyCreatable
{
    Guid CreatedById { get; }
    string? CreatedByName { get; }
    Guid CreatedByTypeId { get; }
    DateTime CreatedOn { get; }
}
