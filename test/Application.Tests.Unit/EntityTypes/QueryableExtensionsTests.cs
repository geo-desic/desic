using AwesomeAssertions;
using Desic.Application.Common.Models;
using Desic.Application.EntityTypes;

namespace Desic.Application.Tests.Unit.EntityTypes;

public class QueryableExtensionsTests
{
    public class QueryableExtensionsTests001 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_NoFilters_AllItemsReturned()
        {
            // arrange
            var expected = GetItems().ToList();
            var items = GetItems();
            var filter = new EntityTypesFilter();

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests002 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<Domain.EntityTypes.EntityType> { expectedItem };
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = expectedItem.Key };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests003 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.EntityTypes.EntityType>();
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = "zzzz" }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests004 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<Domain.EntityTypes.EntityType> { expectedItem };
            var items = GetItems();
            var filter = new EntityTypesFilter { Name = expectedItem.Name };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests005 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByNameThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.EntityTypes.EntityType>();
            var items = GetItems();
            var filter = new EntityTypesFilter { Name = "DoesNotExist" }; // non-existant value

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests006 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyAndNameThatMatchesAnItem_MatchingItemReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<Domain.EntityTypes.EntityType> { expectedItem };
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = expectedItem.Key, Name = expectedItem.Name };

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests007 : QueryableExtensionsTests
    {
        [Fact]
        public void ApplyFilter_FilterByKeyAndNameThatDoesNotMatchAnyItems_NoItemsReturned()
        {
            // arrange
            var expected = new List<Domain.EntityTypes.EntityType>();
            var items = GetItems();
            var filter = new EntityTypesFilter { Key = ItemKey2NameB.Key, Name = ItemKey3NameC.Name }; // each individually matches an item, but none when combined

            // act
            var result = QueryableExtensions.ApplyFilter(source: items.AsQueryable(), filter: filter).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class QueryableExtensionsTests008 : QueryableExtensionsTests
    {
        [Theory]
        [InlineData(null, null)]
        [InlineData(EntityTypesOrderingProperty.Key, true)]
        [InlineData(EntityTypesOrderingProperty.Key, false)]
        [InlineData(EntityTypesOrderingProperty.Name, true)]
        [InlineData(EntityTypesOrderingProperty.Name, false)]
        public void OrderBy_SpecifiedOrderingMethod_OrdersItemsAsExpected(EntityTypesOrderingProperty? property, bool? ascending)
        {
            // arrange
            var expected = GetItemsOrdered(property: property, ascending: ascending).ToList();
            var items = GetItems();
            OrderingMethod<EntityTypesOrderingProperty> orderingMethod;
            if (property.HasValue && ascending.HasValue)
            {
                orderingMethod = new OrderingMethod<EntityTypesOrderingProperty>
                {
                    OrderBy = [new OrderBy<EntityTypesOrderingProperty> { Ascending = ascending.Value, Property = property.Value }],
                };
            }
            else
            {
                orderingMethod = OrderingMethod<EntityTypesOrderingProperty>.Default;
            }

            // act
            var result = QueryableExtensions.OrderBy(source: items.AsQueryable(), orderingMethod: orderingMethod).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableExtensionsTests009 : QueryableExtensionsTests
    {
        [Fact]
        public void SelectToModel_SpecifiedSingleItemQuery_ExpectedModelQueryReturned()
        {
            // arrange
            var expectedItem = ItemKey3NameC;
            var expected = new List<EntityType> { new() { Key = expectedItem.Key, Name = expectedItem.Name } };
            var source = GetItems().AsQueryable().Where(x => x.Key == expectedItem.Key);

            // act
            var result = QueryableExtensions.SelectToModel(source: source).ToList();

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }

    // purposely constructed so that ordering by each property is different
    private static Domain.EntityTypes.EntityType ItemKey1NameE => new() { Key = "key1", Name = "E" };
    private static Domain.EntityTypes.EntityType ItemKey2NameB => new() { Key = "key2", Name = "B" };
    private static Domain.EntityTypes.EntityType ItemKey3NameC => new() { Key = "key3", Name = "C" };
    private static Domain.EntityTypes.EntityType ItemKey4NameA => new() { Key = "key4", Name = "A" };
    private static Domain.EntityTypes.EntityType ItemKey5NameD => new() { Key = "key5", Name = "D" };

    private static IEnumerable<Domain.EntityTypes.EntityType> GetItems()
    {
        // in no particular order
        yield return ItemKey1NameE;
        yield return ItemKey4NameA;
        yield return ItemKey2NameB;
        yield return ItemKey5NameD;
        yield return ItemKey3NameC;
    }

    private static IEnumerable<Domain.EntityTypes.EntityType> GetItemsOrdered(EntityTypesOrderingProperty? property, bool? ascending)
    {
        switch ((property, ascending))
        {
            case (EntityTypesOrderingProperty.Name, false):
                yield return ItemKey1NameE;
                yield return ItemKey5NameD;
                yield return ItemKey3NameC;
                yield return ItemKey2NameB;
                yield return ItemKey4NameA;
                break;
            case (EntityTypesOrderingProperty.Name, _):
                yield return ItemKey4NameA;
                yield return ItemKey2NameB;
                yield return ItemKey3NameC;
                yield return ItemKey5NameD;
                yield return ItemKey1NameE;
                break;
            case (EntityTypesOrderingProperty.Key, false):
                yield return ItemKey5NameD;
                yield return ItemKey4NameA;
                yield return ItemKey3NameC;
                yield return ItemKey2NameB;
                yield return ItemKey1NameE;
                break;
            case (EntityTypesOrderingProperty.Key, _):
                yield return ItemKey1NameE;
                yield return ItemKey2NameB;
                yield return ItemKey3NameC;
                yield return ItemKey4NameA;
                yield return ItemKey5NameD;
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
