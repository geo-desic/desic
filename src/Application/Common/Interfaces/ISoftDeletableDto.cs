using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface ISoftDeletableDto
{
    OptionalOnByType Deleted { get; set; }
}
