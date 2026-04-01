namespace Desic.Domain.Common.Interfaces;

public interface IReadOnlyModifiable
{
    Guid ModifiedById { get; }
    string? ModifiedByName { get; }
    Guid ModifiedByTypeId { get; }
    DateTime ModifiedOn { get; }
}
