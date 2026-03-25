namespace Desic.Application.Common.Interfaces;

public interface IFilterable<out T>
{
    T Filter { get; }
}
