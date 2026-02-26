using AwesomeAssertions;
using Desic.Domain.Common.Entities;
using Desic.Domain.EntityTypes;
using System.Reflection;

namespace Desic.Domain.Tests.Unit;

public class EntityTests
{
    [Fact]
    public void Entity_InheritsFromBaseEntity_EntityTypeSpecified()
    {
        IEnumerable<Type> types = typeof(Domain.IMarker).Assembly.GetTypes();
        var derivedTypes = from type in types where type.IsSubclassOf(typeof(BaseEntity)) && type.IsClass && !type.IsAbstract select type;
        foreach (var derivedType in derivedTypes)
        {
            var propertyInfo = derivedType.GetProperty(nameof(IStaticEntityType.EntityType), BindingFlags.Public | BindingFlags.Static);
            var methodInfo = derivedType.GetMethod(nameof(IReadOnlyMinimalEntity.GetEntityType));
            var instance = Activator.CreateInstance(derivedType);

            var entityTypeClass = propertyInfo?.GetValue(null, null) as IReadOnlyEntityType;
            var entityTypeInstance = methodInfo?.Invoke(instance, null) as IReadOnlyEntityType;

            if (derivedType != typeof(EntityType)) // skipping this one because the above logic doesn't correctly execute the IStaticEntityType.EntityType property due to property name == class name issue
            {
                entityTypeClass.Should().NotBeNull();
                entityTypeClass.Name.Should().NotBe(nameof(SystemEntityType.Unspecified));
            }
            entityTypeInstance.Should().NotBeNull();
            entityTypeInstance.Name.Should().NotBe(nameof(SystemEntityType.Unspecified));
        }
    }
}
