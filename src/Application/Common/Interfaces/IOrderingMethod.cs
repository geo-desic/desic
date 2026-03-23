namespace Desic.Application.Common.Interfaces;

public interface IOrderingMethod<T> where T : struct, Enum
{
    T? OrderingMethod { get; set; }
}
