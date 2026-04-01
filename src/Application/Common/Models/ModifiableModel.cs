using Desic.Application.Common.Extensions;

namespace Desic.Application.Common.Models;

public abstract class ModifiableModel : CreatableModel, Interfaces.IModifiable
{
    protected ModifiableModel() : base() { }
    protected ModifiableModel(Domain.Common.Interfaces.IReadOnlyModifiableEntity from) : base(from)
    {
        this.MapModified(from);
    }
    public RequiredOnByType Modified { get; set; } = new();
}
