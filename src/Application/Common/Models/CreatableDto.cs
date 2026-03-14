using Desic.Application.Common.Extensions;
using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class CreatableDto : BaseDto, ICreatableDto
{
    public CreatableDto() : base() { }
    public CreatableDto(CreatableEntity entity) : base(entity)
    {
        this.MapCreated(entity);
    }
    public RequiredOnByType Created { get; set; } = new();
}
