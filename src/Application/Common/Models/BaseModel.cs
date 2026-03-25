using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class BaseModel
{
    public BaseModel() { }
    public BaseModel(BaseEntity entity)
    {
        Id = entity.Id;
    }
    public Guid Id { get; set; }
}
