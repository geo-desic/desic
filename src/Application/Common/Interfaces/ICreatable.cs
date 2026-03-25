using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface ICreatable
{
    RequiredOnByType Created { get; set; }
}
