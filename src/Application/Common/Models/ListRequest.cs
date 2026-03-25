using Desic.Application.Common.Interfaces;

namespace Desic.Application.Common.Models;

public class ListRequest : IListRequest
{
    public IPagination Pagination { get; set; } = new Pagination();
}
