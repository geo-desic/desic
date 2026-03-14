using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface IModifiableDto
{
    RequiredOnByType Modified { get; set; }
}
