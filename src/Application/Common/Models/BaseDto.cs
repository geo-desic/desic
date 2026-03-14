using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class BaseDto
{
    public BaseDto() { }
    public BaseDto(BaseEntity entity)
    {
        Id = entity.Id;
    }
    public Guid Id { get; set; }
}
