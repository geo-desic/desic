namespace Desic.Domain.Common.Entities;

public interface IUpdatableFrom<T>
{
    void UpdateFrom(T compare);
}
