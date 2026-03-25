using AwesomeAssertions;
using Desic.Application.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.EntityTypes;

public class EntityTypeExtensionsTests
{
    public class EntityTypeExtensionsTests001 : EntityTypeExtensionsTests
    {
        [Fact]
        public void ToModel_AllPropertiesSpecified_ItemWithAllExpectedPropertyValuesReturned()
        {
            // arrange
            var source = new Domain.EntityTypes.EntityType
            {
                Id = 1.ToGuid(),
                Key = "key1",
                Name = "ExpectedName",
            };
            var expected = new EntityType
            {
                Key = source.Key,
                Name = source.Name,
            };

            // act
            var result = EntityTypeExtensions.ToModel(source: source);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
