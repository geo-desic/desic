namespace Desic.Domain.Common.Interfaces;

public interface IUpdatableFrom<T>
{
    void UpdateFrom(T compare);
}
