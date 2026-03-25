namespace Desic.Application.EntityTypes;

public static class EntityTypeExtensions
{
    public static EntityType ToDto(this Domain.EntityTypes.EntityType source)
    {
        return new EntityType
        {
            Key = source.Key,
            Name = source.Name,
        };
    }
}
