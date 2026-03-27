using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.Common.Entities;

public abstract class CreatableEntity : BaseEntity, ICreatable
{
    public Guid CreatedById { get; set; }
    public string? CreatedByName { get; set; }
    public Guid CreatedByTypeId { get; set; }
    public DateTime CreatedOn { get; set; }

    public const int MaxLengthCreatedByName = 100;
}
