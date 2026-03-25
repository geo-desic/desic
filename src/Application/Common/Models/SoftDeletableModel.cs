using Desic.Application.Common.Extensions;
using Desic.Domain.Common.Entities;

namespace Desic.Application.Common.Models;

public abstract class SoftDeletableModel : ModifiableModel, Interfaces.ISoftDeletable
{
    protected SoftDeletableModel() : base() { }
    protected SoftDeletableModel(SoftDeletableEntity entity) : base(entity)
    {
        this.MapDeleted(entity);
    }
    public OptionalOnByType Deleted { get; set; } = new();
}
