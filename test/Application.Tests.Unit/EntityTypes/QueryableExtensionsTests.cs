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
        public void OrderBy_SpecifiedOrderingMethod_OrdersItemsAsExpected(EntityTypesOrderingMethod? orderingMethod)
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

    public class QueryableExtensionsTests002 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_NoFilters_AllItemsReturned ()
        {
            // arrange
            var expected = GetItems().ToList();
            var items = GetItems();
            var filter = new EntityTypesFilter();

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests003 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<Domain.EntityTypes.EntityType>() { expectedItem };
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = expectedItem.Key };

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests004 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyThatDoesMatchAnItem_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.EntityTypes.EntityType>();
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = "zzzz" }; // non-existant key

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests005 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<Domain.EntityTypes.EntityType>() { expectedItem };
            var items = GetItems();
            var filter = new EntityTypesFilter { Name = expectedItem.Name };

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests006 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameThatDoesMatchAnItem_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.EntityTypes.EntityType>();
            var items = GetItems();
            var filter = new EntityTypesFilter { Name = "DoesNotExist" }; // non-existant name

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests007 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyAndNameThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<Domain.EntityTypes.EntityType>() { expectedItem };
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = expectedItem.Key, Name = expectedItem.Name };

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests008 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyAndNameThatDoesNotMatchAnItem_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.EntityTypes.EntityType>();
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = ItemKey2NameB.Key, Name = ItemKey3NameC.Name }; // each individually matches an item, but none when combined

            // act
            var result = QueryableExtensions.ApplyFilter(query: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    // purposely constructed so that ordering by key is different than ordering by name
    private static Domain.EntityTypes.EntityType ItemKey1NameE => new() { Key = "key1", Name = "E" };
    private static Domain.EntityTypes.EntityType ItemKey2NameB => new() { Key = "key2", Name = "B" };
    private static Domain.EntityTypes.EntityType ItemKey3NameC => new() { Key = "key3", Name = "C" };
    private static Domain.EntityTypes.EntityType ItemKey4NameA => new() { Key = "key4", Name = "A" };
    private static Domain.EntityTypes.EntityType ItemKey5NameD => new() { Key = "key5", Name = "D" };

    private static IEnumerable<Domain.EntityTypes.EntityType> GetItems()
    {
        // purposely in jumbled order
        yield return ItemKey1NameE;
        yield return ItemKey4NameA;
        yield return ItemKey2NameB;
        yield return ItemKey5NameD;
        yield return ItemKey3NameC;
    }

    private static IEnumerable<Domain.EntityTypes.EntityType> GetItemsOrdered(EntityTypesOrderingMethod? orderingMethod)
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
