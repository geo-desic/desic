using AwesomeAssertions;
using Desic.Application.EntityTypes;

namespace Desic.Application.Tests.Unit.EntityTypes;

public class QueryableExtensionsTests
{
    public class QueryableExtensionsTests001 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData(EntityTypesOrderingMethod.KeyAsc)]
        [InlineData(EntityTypesOrderingMethod.KeyDesc)]
        [InlineData(EntityTypesOrderingMethod.NameAsc)]
        [InlineData(EntityTypesOrderingMethod.NameDesc)]
        public void OrderBy_SpecifiedOrderingMethod_OrdersResultsAsExpected(EntityTypesOrderingMethod? orderingMethod)
        {
            // arrange
            var expected = GetItemsOrdered(orderingMethod).ToList();
            var items = GetItems();

            // act
            var result = QueryableExtensions.OrderBy(query: items.AsQueryable(), orderingMethod: orderingMethod).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    // purposely constructed so that ordering by key is different than ordering by name
    private static EntityType ItemKey1NameE => new() { Key = "key1", Name = "E" };
    private static EntityType ItemKey2NameB => new() { Key = "key2", Name = "B" };
    private static EntityType ItemKey3NameC => new() { Key = "key3", Name = "C" };
    private static EntityType ItemKey4NameA => new() { Key = "key4", Name = "A" };
    private static EntityType ItemKey5NameD => new() { Key = "key5", Name = "D" };

    private static IEnumerable<EntityType> GetItems()
    {
        // purposely in jumbled order
        yield return ItemKey1NameE;
        yield return ItemKey4NameA;
        yield return ItemKey2NameB;
        yield return ItemKey5NameD;
        yield return ItemKey3NameC;
    }

    private static IEnumerable<EntityType> GetItemsOrdered(EntityTypesOrderingMethod? orderingMethod)
    {
        switch (orderingMethod)
        {
            case EntityTypesOrderingMethod.NameAsc:
                yield return ItemKey4NameA;
                yield return ItemKey2NameB;
                yield return ItemKey3NameC;
                yield return ItemKey5NameD;
                yield return ItemKey1NameE;
                break;
            case EntityTypesOrderingMethod.NameDesc:
                yield return ItemKey1NameE;
                yield return ItemKey5NameD;
                yield return ItemKey3NameC;
                yield return ItemKey2NameB;
                yield return ItemKey4NameA;
                break;
            case EntityTypesOrderingMethod.KeyAsc:
                yield return ItemKey1NameE;
                yield return ItemKey2NameB;
                yield return ItemKey3NameC;
                yield return ItemKey4NameA;
                yield return ItemKey5NameD;
                break;
            case EntityTypesOrderingMethod.KeyDesc:
                yield return ItemKey5NameD;
                yield return ItemKey4NameA;
                yield return ItemKey3NameC;
                yield return ItemKey2NameB;
                yield return ItemKey1NameE;
                break;
            default: // default ordering method is name ascending
                yield return ItemKey4NameA;
                yield return ItemKey2NameB;
                yield return ItemKey3NameC;
                yield return ItemKey5NameD;
                yield return ItemKey1NameE;
                break;
        }
    }
}
