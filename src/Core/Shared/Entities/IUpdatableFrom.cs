namespace Desic.Core.Shared.Entities;

public interface IUpdatableFrom<T>
{
    void UpdateFrom(T compare);
}
