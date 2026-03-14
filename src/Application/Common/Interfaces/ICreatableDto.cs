using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface ICreatableDto
{
    RequiredOnByType Created { get; set; }
}
