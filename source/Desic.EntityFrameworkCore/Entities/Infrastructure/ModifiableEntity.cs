namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

public abstract class ModifiableEntity : CreatableEntity, ICreatableModifiable
{
    public Guid ModifiedById { get; set; }
    public Guid ModifiedByTypeId { get; set; }
    public DateTime ModifiedOn { get; set; }
}
