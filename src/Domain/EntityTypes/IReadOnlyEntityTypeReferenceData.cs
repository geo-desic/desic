namespace Desic.Domain.EntityTypes;

public interface IReadOnlyEntityTypeReferenceData : IEquatable<IReadOnlyEntityTypeReferenceData>
{
    string Key { get; }
    string Name { get; }
}
