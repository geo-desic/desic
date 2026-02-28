using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using System.Reflection;

namespace Desic.Domain.Tests.Unit;

public class EntityTests
{
    [Fact]
    public void Entity_InheritsFromBaseEntity_EntityTypeSpecifiedAndUnique()
    {
        List<SystemEntityType> entityTypes = [];
        IEnumerable<Type> types = typeof(Domain.IMarker).Assembly.GetTypes();
        var derivedTypes = from type in types where type.IsSubclassOf(typeof(BaseEntity)) && type.IsClass && !type.IsAbstract select type;
        foreach (var derivedType in derivedTypes)
        {
            var staticPropertyInfo = derivedType.GetProperty(nameof(IStaticEntityType.ClassEntityType), BindingFlags.Public | BindingFlags.Static);
            var propertyInfo = derivedType.GetProperty(nameof(IReadOnlyMinimalEntity.SystemEntityType));
            var instance = Activator.CreateInstance(derivedType);

            var entityTypeClass = staticPropertyInfo?.GetValue(null, null) as SystemEntityType;
            var entityTypeInstance = propertyInfo?.GetValue(instance, null) as SystemEntityType;

            entityTypeClass.Should().NotBeNull();
            entityTypeClass.Should().NotBe(SystemEntityTypes.Unspecified);
            entityTypeInstance.Should().NotBeNull();
            entityTypeInstance.Should().Be(entityTypeClass);
            entityTypes.Should().NotContain(entityTypeInstance);
            entityTypes.Add(entityTypeInstance);
        }
    }
}
