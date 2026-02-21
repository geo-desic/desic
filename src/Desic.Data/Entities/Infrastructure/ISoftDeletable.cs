namespace Desic.Data.Entities.Infrastructure;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
    Guid? DeletedById { get; set; }
    Guid? DeletedByTypeId { get; set; }
    DateTime? DeletedOn { get; set; }
}
