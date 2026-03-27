using AwesomeAssertions;
using Desic.Application.EntityTypes;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.EntityTypes;

public class EntityTypeTests
{
    public class EntityTypeTests001 : EntityTypeTests
    {
        [Fact]
        public void Constructor_WithDomainEntity_AllPropertiesMappedCorrectly()
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
            var result = new EntityType(source: source);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
