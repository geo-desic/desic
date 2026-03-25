using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public abstract class BaseModel
{
    protected BaseModel() { }
    protected BaseModel(BaseEntity entity)
    {
        Id = entity.Id;
    }
    public Guid Id { get; set; }
}
