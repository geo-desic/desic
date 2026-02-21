namespace Desic.Data.Entities.Infrastructure;

public interface IUpdatableFrom<T>
{
    void UpdateFrom(T compare);
}
