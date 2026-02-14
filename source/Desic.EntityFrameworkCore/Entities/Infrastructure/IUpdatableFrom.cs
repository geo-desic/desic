namespace Desic.EntityFrameworkCore.Entities.Infrastructure;

public interface IUpdatableFrom<T>
{
    void UpdateFrom(T compare);
}
