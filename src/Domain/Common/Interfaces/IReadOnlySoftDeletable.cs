namespace Desic.Domain.Common.Interfaces;

public interface IReadOnlySoftDeletable
{
    Guid? DeletedById { get; }
    string? DeletedByName { get; }
    Guid? DeletedByTypeId { get; }
    DateTime? DeletedOn { get; }
    bool IsDeleted { get; }
}
