namespace Desic.Domain.Common.Interfaces;

public interface ICreatable
{
    Guid CreatedById { get; set; }
    string? CreatedByName { get; set; }
    Guid CreatedByTypeId { get; set; }
    DateTime CreatedOn { get; set; }
}
