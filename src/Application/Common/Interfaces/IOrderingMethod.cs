namespace Desic.Application.Common.Interfaces;

public interface IOrderingMethod<T> : IOrderingMethod where T : struct, Enum
{
    new IReadOnlyList<IOrderBy<T>> OrderBy { get; }
    IReadOnlyList<IOrderBy> IOrderingMethod.OrderBy { get => OrderBy; }
}

public interface IOrderingMethod
{
    IReadOnlyList<IOrderBy> OrderBy { get; }
}
