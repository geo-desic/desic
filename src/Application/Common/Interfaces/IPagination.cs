namespace Desic.Application.Common.Interfaces;

public interface IPagination
{
    int Count { get; set; }
    bool IncludeTotalCount { get; set; }
    int StartIndex { get; set; }
}
