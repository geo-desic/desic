using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class ListRequest : IListRequest
{
    public int? Count { get; set; }
    public int? StartIndex { get; set; }
}
