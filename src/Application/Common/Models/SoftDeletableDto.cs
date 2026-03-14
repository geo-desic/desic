using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class SoftDeletableDto : ModifiableDto, ISoftDeleted
{
    public OptionalOnByType Deleted { get; set; } = new();
}
