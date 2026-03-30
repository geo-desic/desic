namespace Desic.Application.Common.Interfaces;

public interface IOrderBy<T> where T : struct, Enum
{
    bool Ascending { get; set; }
    T Property { get; set; }
}
