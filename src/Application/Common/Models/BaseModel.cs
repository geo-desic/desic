using Desic.Application.Common.Extensions;
using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public abstract class BaseModel : IGuidId
{
    protected BaseModel() { }
    protected BaseModel(BaseEntity entity)
    {
        this.MapId(entity);
    }
    public Guid Id { get; set; }
}
