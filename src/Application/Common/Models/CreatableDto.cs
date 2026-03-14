using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class CreatableDto : BaseDto, ICreated
{
    public RequiredOnByType Created { get; set; } = new();
}
