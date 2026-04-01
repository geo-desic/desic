using Desic.Application.Common.Extensions;
using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public abstract class BaseModel : IGuidId
{
    protected BaseModel() { }
    protected BaseModel(Domain.Common.Interfaces.IReadOnlyGuidId from)
    {
        this.MapId(from);
    }
    public Guid Id { get; set; }
}
