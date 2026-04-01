using Desic.Application.Common.Extensions;

namespace Desic.Application.Common.Models;

public abstract class CreatableModel : BaseModel, Interfaces.ICreatable
{
    protected CreatableModel() : base() { }
    protected CreatableModel(Domain.Common.Interfaces.IReadOnlyCreatableEntity from) : base(from)
    {
        this.MapCreated(from);
    }
    public RequiredOnByType Created { get; set; } = new();
}
