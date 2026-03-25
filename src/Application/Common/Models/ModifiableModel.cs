using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class ModifiableModel : CreatableModel, Interfaces.IModifiable
{
    public ModifiableModel() : base() { }
    public ModifiableModel(ModifiableEntity entity) : base(entity)
    {
        this.MapModified(entity);
    }
    public RequiredOnByType Modified { get; set; } = new();
}
