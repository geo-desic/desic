using AwesomeAssertions;
using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Models;
using Desic.Shared.Extensions;

namespace Desic.Application.Tests.Unit.Common.Infrastructure;

public class EnumQueryableOrdererTests
{
    public class EnumQueryableOrdererTests001 : EnumQueryableOrdererTests
    {
        [Fact]
        public void ApplyOrderingMethod_OrderByIsEmpty_ThrowsArgumentException()
        {
            // arrange
            var orderer = new TestEnumQueryableOrderer();
            var orderingMethod = new OrderingMethod<TestSourceOrderingProperty> { OrderBy = [] }; // problematic ====> OrderBy must have at least 1 item
            var act = () => orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod);

            // act & assert
            act.Should().Throw<ArgumentException>();
        }
    }

    public class EnumQueryableOrdererTests002 : EnumQueryableOrdererTests
    {
        [Fact]
        public void ApplyOrderingMethod_OrderByHasMoreItemsThanAllowed_ThrowsArgumentException()
        {
            // arrange
            var orderer = new TestEnumQueryableOrderer();
            var orderingMethod = new OrderingMethod<TestSourceOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestSourceOrderingProperty.Unique },
                    new() { Property = TestSourceOrderingProperty.NonUniqueString },
                    new() { Property = TestSourceOrderingProperty.NonUniqueDateTime },
                    new() { Property = TestSourceOrderingProperty.NonUniqueInteger }, // problematic ====> OrderBy cannot have more than 3 items
                ],
            };
            var act = () => orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod);

            // act & assert
            act.Should().Throw<ArgumentException>();
        }
    }

    public class EnumQueryableOrdererTests003 : EnumQueryableOrdererTests
    {
        [Fact]
        public void ApplyOrderingMethod_OrderByHasUnmappedProperty_ThrowsArgumentException()
        {
            // arrange
            var orderer = new TestEnumQueryableOrderer();
            var orderingMethod = new OrderingMethod<TestSourceOrderingProperty>
            {
                OrderBy = [new() { Property = TestSourceOrderingProperty.Unmapped }],
            };
            var act = () => orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod);

            // act & assert
            act.Should().Throw<NotSupportedException>();
        }
    }

    public class EnumQueryableOrdererTests004 : EnumQueryableOrdererTests
    {
        [Theory]
        // ascending = true: the following 3 test cases all order the same way since the first item in OrderBy has all unique values (so the additional ordering properties do not have any impact)
        [InlineData(true, null, null)]
        [InlineData(true, TestSourceOrderingProperty.NonUniqueString, null)]
        [InlineData(true, TestSourceOrderingProperty.NonUniqueString, TestSourceOrderingProperty.NonUniqueDateTime)]
        // ascending = false: the following 3 test cases all order the same way since the first item in OrderBy has all unique values (so the additional ordering properties do not have any impact)
        [InlineData(false, null, null)]
        [InlineData(false, TestSourceOrderingProperty.NonUniqueString, null)]
        [InlineData(false, TestSourceOrderingProperty.NonUniqueString, TestSourceOrderingProperty.NonUniqueDateTime)]
        public void ApplyOrderingMethod_FirstOrderByItemPropertyHasAllUniqueValues_ResultingOrderingCompletelyBasedOnUniqueValuesAndFirstItemAscendingValue(bool ascending, TestSourceOrderingProperty? additionalProperty1 = null, TestSourceOrderingProperty? additionalProperty2 = null)
        {
            // arrange
            var orderer = new TestEnumQueryableOrderer();
            var orderingMethod = new OrderingMethod<TestSourceOrderingProperty>
            {
                OrderBy = [new() { Property = TestSourceOrderingProperty.Unique, Ascending = ascending }],
            };
            if (additionalProperty1 != null) orderingMethod.OrderBy.Add(new() { Property = additionalProperty1.Value });
            if (additionalProperty2 != null) orderingMethod.OrderBy.Add(new() { Property = additionalProperty2.Value });
            var expected = GetItemsOrderedByUnique(ascending).ToList();

            // act
            var result = orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class EnumQueryableOrdererTests005 : EnumQueryableOrdererTests
    {
        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ApplyOrderingMethod_FirstTwoOrderByItemsPropertyValuesDoNotResultingInUniqueOrdering_3rdItemValuesAndAscendingValueHaveAnImpactOnResultingOrdering(bool asendingUnique)
        {
            // arrange
            var orderer = new TestEnumQueryableOrderer();
            var orderingMethod = new OrderingMethod<TestSourceOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestSourceOrderingProperty.NonUniqueString, Ascending = true },
                    new() { Property = TestSourceOrderingProperty.NonUniqueDateTime, Ascending = false },
                    new() { Property = TestSourceOrderingProperty.Unique, Ascending = asendingUnique },
                ],
            };
            var expected = GetItemsOrderedByNonUniqueStringAscNonUniqueDateTimeDescUnique(asendingUnique).ToList();

            // act
            var result = orderer.ApplyOrderingMethod(GetItems().AsQueryable(), orderingMethod).ToList();

            // assert
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    // purposely in no particular order
    private static IEnumerable<TestSource> GetItems()
    {
        yield return ItemU2NsBNd2025;
        yield return ItemU5NsANdNull;
        yield return ItemU3NsANd2025;
        yield return ItemU4NsBNd2020;
        yield return ItemU1NsANd2025;
    }

    private static IEnumerable<TestSource> GetItemsOrderedByUnique(bool ascending)
    {
        if (ascending)
        {
            yield return ItemU1NsANd2025;
            yield return ItemU2NsBNd2025;
            yield return ItemU3NsANd2025;
            yield return ItemU4NsBNd2020;
            yield return ItemU5NsANdNull;
        }
        else
        {
            yield return ItemU5NsANdNull;
            yield return ItemU4NsBNd2020;
            yield return ItemU3NsANd2025;
            yield return ItemU2NsBNd2025;
            yield return ItemU1NsANd2025;
        }
    }

    private static IEnumerable<TestSource> GetItemsOrderedByNonUniqueStringAscNonUniqueDateTimeDescUnique(bool ascending)
    {
        if (ascending)
        {
            yield return ItemU1NsANd2025; // "A", 2025, 1
            yield return ItemU3NsANd2025; // "A", 2025, 3
        }
        else
        {
            yield return ItemU3NsANd2025; // "A", 2025, 3
            yield return ItemU1NsANd2025; // "A", 2025, 1
        }
        yield return ItemU5NsANdNull; // "A", null, 5
        yield return ItemU2NsBNd2025; // "B", 2025, 2
        yield return ItemU4NsBNd2020; // "B", 2020, 4
    }

    private readonly static TestSource ItemU1NsANd2025 = new() { Unique = 1.ToGuid(), NonUniqueString = "A", NonUniqueDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = null, NonOrderableProperty = $"{nameof(TestSource.NonOrderableProperty)}Value1" };
    private readonly static TestSource ItemU2NsBNd2025 = new() { Unique = 2.ToGuid(), NonUniqueString = "B", NonUniqueDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = null }; // all orderable property values same as above except for Unique
    private readonly static TestSource ItemU3NsANd2025 = new() { Unique = 3.ToGuid(), NonUniqueString = "A", NonUniqueDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = null };
    private readonly static TestSource ItemU4NsBNd2020 = new() { Unique = 4.ToGuid(), NonUniqueString = "B", NonUniqueDateTime = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = 1 };
    private readonly static TestSource ItemU5NsANdNull = new() { Unique = 5.ToGuid(), NonUniqueString = "A", NonUniqueDateTime = null, NonUniqueInteger = 1 };

    public enum TestSourceOrderingProperty
    {
        Unique,
        NonUniqueString,
        NonUniqueDateTime,
        NonUniqueInteger,
        Unmapped,
    }

    private class TestSource
    {
        public Guid Unique { get; set; }
        public required string NonUniqueString { get; set; }
        public DateTime? NonUniqueDateTime { get; set; }
        public int? NonUniqueInteger { get; set; }
        public string? NonOrderableProperty { get; set; }
    }

    private class TestEnumQueryableOrderer : EnumQueryableOrderer<TestSourceOrderingProperty, TestSource>
    {
        public TestEnumQueryableOrderer()
        {
            Map(TestSourceOrderingProperty.Unique, s => s.Unique);
            Map(TestSourceOrderingProperty.NonUniqueString, s => s.NonUniqueString);
            Map(TestSourceOrderingProperty.NonUniqueDateTime, s => s.NonUniqueDateTime);
            Map(TestSourceOrderingProperty.NonUniqueInteger, s => s.NonUniqueInteger);
            // Unmapped is purposely not mapped to assert that an exception is thrown when trying to order by it
        }
    }
}
