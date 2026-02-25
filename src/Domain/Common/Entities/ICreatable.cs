namespace Desic.Domain.Common.Entities;

public interface ICreatable
{
    Guid CreatedById { get; set; }
    Guid CreatedByTypeId { get; set; }
    DateTime CreatedOn { get; set; }
}
