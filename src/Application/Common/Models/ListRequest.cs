using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Desic.Application.Common.Models;

public class ListRequest : IListRequest
{
    [Description("The requested count of items to be returned that may not be honored if it is larger than the maximum count allowed")]
    [DefaultValue(ListRequests.DefaultCount)]
    [Range(minimum: 0, maximum: int.MaxValue)]
    public int Count { get; set; } = ListRequests.DefaultCount;

    [Description("The requested start index applied after items are queried and ordered that can be used for pagination")]
    [DefaultValue(ListRequests.DefaultStartIndex)]
    [Range(minimum: 0, maximum: int.MaxValue)]
    public int StartIndex { get; set; } = ListRequests.DefaultStartIndex;
}
