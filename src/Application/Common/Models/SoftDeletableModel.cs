using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public class SoftDeletableModel : ModifiableModel, Interfaces.ISoftDeletable
{
    public SoftDeletableModel() : base() { }
    public SoftDeletableModel(SoftDeletableEntity entity) : base(entity)
    {
        this.MapDeleted(entity);
    }
    public OptionalOnByType Deleted { get; set; } = new();
}
