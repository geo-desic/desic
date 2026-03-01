using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface IModified
{
    RequiredOnByType Modified { get; set; }
}
