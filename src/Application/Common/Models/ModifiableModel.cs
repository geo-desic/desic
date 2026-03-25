using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public abstract class ModifiableModel : CreatableModel, Interfaces.IModifiable
{
    protected ModifiableModel() : base() { }
    protected ModifiableModel(ModifiableEntity entity) : base(entity)
    {
        this.MapModified(entity);
    }
    public RequiredOnByType Modified { get; set; } = new();
}
