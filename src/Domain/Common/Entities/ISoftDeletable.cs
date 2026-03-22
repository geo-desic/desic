namespace Desic.Domain.Common.Entities;

public interface ISoftDeletable
{
    Guid? DeletedById { get; set; }
    string? DeletedByName { get; set; }
    Guid? DeletedByTypeId { get; set; }
    DateTime? DeletedOn { get; set; }
    bool IsDeleted { get; }
}
