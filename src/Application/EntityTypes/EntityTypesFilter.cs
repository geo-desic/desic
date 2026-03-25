using Desic.Application.Common.Infrastructure;
using System.ComponentModel;

namespace Desic.Application.EntityTypes;

public class EntityTypesFilter
{
    [Description(Filters.DescriptionNonNullExact)]
    public string? Key { get; set; }

    [Description(Filters.DescriptionNonNullExact)]
    public string? Name { get; set; }
}
