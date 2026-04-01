namespace Desic.Domain.Common.Interfaces;

public interface ICreatable : IReadOnlyCreatable
{
    new Guid CreatedById { get; set; }
    new string? CreatedByName { get; set; }
    new Guid CreatedByTypeId { get; set; }
    new DateTime CreatedOn { get; set; }
}
