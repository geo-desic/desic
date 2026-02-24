namespace Desic.Core.Shared.Entities;

public abstract class ModifiableEntity : CreatableEntity, IModifiable
{
    public Guid ModifiedById { get; set; }
    public Guid ModifiedByTypeId { get; set; }
    public DateTime ModifiedOn { get; set; }
}
