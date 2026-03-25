using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class CreatableModel : BaseModel, Interfaces.ICreatable
{
    public CreatableModel() : base() { }
    public CreatableModel(CreatableEntity entity) : base(entity)
    {
        this.MapCreated(entity);
    }
    public RequiredOnByType Created { get; set; } = new();
}
