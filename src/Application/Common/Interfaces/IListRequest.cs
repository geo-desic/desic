namespace Desic.Application.Common.Interfaces;

public interface IListRequest
{
    int? Count { get; set; }
    int? StartIndex { get; set; }
}
