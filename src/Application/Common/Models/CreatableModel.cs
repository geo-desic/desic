using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public abstract class CreatableModel : BaseModel, Interfaces.ICreatable
{
    protected CreatableModel() : base() { }
    protected CreatableModel(CreatableEntity entity) : base(entity)
    {
        this.MapCreated(entity);
    }
    public RequiredOnByType Created { get; set; } = new();
}
