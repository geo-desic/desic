using Desic.Domain.Common.Interfaces;

namespace Desic.Domain.Common.Entities;

public abstract class ModifiableEntity : CreatableEntity, IModifiable
{
    public Guid ModifiedById { get; set; }
    public string? ModifiedByName { get; set; }
    public Guid ModifiedByTypeId { get; set; }
    public DateTime ModifiedOn { get; set; }

    public const int MaxLengthModifiedByName = 100;
}
