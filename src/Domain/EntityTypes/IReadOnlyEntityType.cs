namespace Desic.Domain.EntityTypes;

public interface IReadOnlyEntityType
{
    string Key { get; }
    string Name { get; }
}
