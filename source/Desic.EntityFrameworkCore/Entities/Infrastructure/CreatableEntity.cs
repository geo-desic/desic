namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

public abstract class CreatableEntity : MinimalEntity, ICreatable
{
    public Guid CreatedById { get; set; }
    public Guid CreatedByTypeId { get; set; }
    public DateTime CreatedOn { get; set; }
}
