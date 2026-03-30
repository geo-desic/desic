namespace Desic.Application.Common.Interfaces;

public interface IOrderingMethod<T> where T : struct, Enum
{
    IReadOnlyList<IOrderBy<T>> OrderBy { get; }
}
