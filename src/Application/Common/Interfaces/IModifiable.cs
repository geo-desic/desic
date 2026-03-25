using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface IModifiable
{
    RequiredOnByType Modified { get; set; }
}
