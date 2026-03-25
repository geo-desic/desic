using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Desic.Application.Common.Models;

public class Pagination : IPagination
{
    [Description("The requested count of items to be returned that is not guaranteed to be honored by the server if it is too large")]
    [DefaultValue(ListRequests.DefaultCount)]
    [Range(minimum: 0, maximum: int.MaxValue)]
    public int Count { get; set; } = ListRequests.DefaultCount;

    [Description("The requested inclusion or omiitance of the total count of items matching the query after applying any filters but before pagination that is not guaranteed to be honored by the server")]
    [DefaultValue(ListRequests.DefaultIncludeTotalCount)]
    public bool IncludeTotalCount { get; set; } = ListRequests.DefaultIncludeTotalCount;

    [Description("The requested start index applied after items are queried and ordered that can be used for pagination")]
    [DefaultValue(ListRequests.DefaultStartIndex)]
    [Range(minimum: 0, maximum: int.MaxValue)]
    public int StartIndex { get; set; } = ListRequests.DefaultStartIndex;
}
