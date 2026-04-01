namespace Desic.Domain.EntityTypes;

public interface IEntityType : IReadOnlyEntityType
{
    new string Key { get; set; }
    new string Name { get; set; }
}
