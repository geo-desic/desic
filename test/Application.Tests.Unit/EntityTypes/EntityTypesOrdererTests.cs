using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;
using Desic.Domain.EntityTypes;

namespace Desic.Application.Tests.Unit.EntityTypes;

// note: most functionality is already tested in the base class, so testing only a few things here such as all properties are mapped properly
public class EntityTypesOrdererTests
{
    public class EntityTypesOrdererTests001 : EntityTypesOrdererTests
    {
        [Fact]
        public void Constructor_ApplyOrderingMethodCalledOnConstructedObjectForAllPossibleEnumValues_AllEnumValuesAreMapped()
        {
            // arrange
            var orderer = new EntityTypesOrderer<TestItem>();
            var expected = GetItems().ToList();

            foreach (var value in Enum.GetValues<EntityTypesOrderingProperty>())
            {
                var orderingMethod = new OrderingMethod<EntityTypesOrderingProperty> { OrderBy = [new() { Property = value }] };

                // act ====> should not throw any exceptions
                var result = orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod).ToList();

                // assert
                result.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering());
            }
        }
    }

    private static IEnumerable<TestItem> GetItems()
    {
        yield return new() { Key = "aaaa", Name = "A" };
    }

    private class TestItem : IReadOnlyEntityType
    {
        public required string Key { get; init; }
        public required string Name { get; init; }
    }
}
