namespace Desic.Domain.EntityTypes;

public interface IReadOnlyEntityType : IEquatable<IReadOnlyEntityType>
{
    string Key { get; }
    string Name { get; }
}
