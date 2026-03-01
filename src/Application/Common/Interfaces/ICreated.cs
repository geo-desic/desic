using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface ICreated
{
    RequiredOnByType Created { get; set; }
}
