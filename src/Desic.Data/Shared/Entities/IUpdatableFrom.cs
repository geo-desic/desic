namespace Desic.Data.Shared.Entities;

public interface IUpdatableFrom<T>
{
    void UpdateFrom(T compare);
}
