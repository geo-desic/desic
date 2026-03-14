using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class ModifiableDto : CreatableDto, IModified
{
    public RequiredOnByType Modified { get; set; } = new();
}
