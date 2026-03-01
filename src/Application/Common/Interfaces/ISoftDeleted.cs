using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface ISoftDeleted
{
    OptionalOnByType Deleted { get; set; }
}
