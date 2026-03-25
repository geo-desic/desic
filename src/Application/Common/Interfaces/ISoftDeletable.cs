using Desic.Application.Common.Models;

namespace Desic.Application.Common.Interfaces;

internal interface ISoftDeletable
{
    OptionalOnByType Deleted { get; set; }
}
