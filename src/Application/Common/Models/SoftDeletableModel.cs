using Desic.Application.Common.Extensions;

namespace Desic.Application.Common.Models;

public abstract class SoftDeletableModel : ModifiableModel, Interfaces.ISoftDeletable
{
    protected SoftDeletableModel() : base() { }
    protected SoftDeletableModel(Domain.Common.Interfaces.IReadOnlySoftDeletableEntity from) : base(from)
    {
        this.MapDeleted(from);
    }
    public OptionalOnByType Deleted { get; set; } = new();
}
