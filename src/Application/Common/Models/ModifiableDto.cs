using Desic.Application.Common.Helpers;
using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class ModifiableDto : CreatableDto, IModifiableDto
{
    public ModifiableDto() : base() { }
    public ModifiableDto(ModifiableEntity entity) : base(entity)
    {
        this.MapModified(entity);
    }
    public RequiredOnByType Modified { get; set; } = new();
}
