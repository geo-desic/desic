using Desic.Application.Common.Helpers;
using Desic.Application.Common.Interfaces;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class SoftDeletableDto : ModifiableDto, ISoftDeletableDto
{
    public SoftDeletableDto() : base() { }
    public SoftDeletableDto(SoftDeletableEntity entity) : base(entity)
    {
        this.MapDeleted(entity);
    }
    public OptionalOnByType Deleted { get; set; } = new();
}
