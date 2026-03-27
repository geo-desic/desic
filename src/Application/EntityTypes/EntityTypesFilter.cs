using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Models;
using System.ComponentModel;

namespace Desic.Application.EntityTypes;

public class EntityTypesFilter : Filter
{
    [Description(Filters.DescriptionNonNullExact)]
    public string? Key { get; set; }

    [Description(Filters.DescriptionNonNullExact)]
    public string? Name { get; set; }
}
