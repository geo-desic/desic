namespace Desic.Data.Shared.Entities;

public interface ICreatable
{
    Guid CreatedById { get; set; }
    Guid CreatedByTypeId { get; set; }
    DateTime CreatedOn { get; set; }
}
