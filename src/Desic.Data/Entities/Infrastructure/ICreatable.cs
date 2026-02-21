namespace Desic.Data.Entities.Infrastructure;

public interface ICreatable
{
    Guid CreatedById { get; set; }
    Guid CreatedByTypeId { get; set; }
    DateTime CreatedOn { get; set; }
}
