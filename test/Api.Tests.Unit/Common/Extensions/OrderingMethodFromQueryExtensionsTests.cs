using AwesomeAssertions;
using Desic.Api.Common.Extensions;
using Desic.Api.Dtos;
using Desic.Application.Common.Models;

namespace Desic.Api.Tests.Unit.Common.Extensions;

public class OrderingMethodFromQueryExtensionsTests
{
    public class OrderingMethodFromQueryExtensionsTests001 : OrderingMethodFromQueryExtensionsTests
    {
        [Fact]
        public void TryConvertToOrderingMethod_ValidInputZeroCount_ReturnsTrueWithDefaultResult()
        {
            // arrange
            var source = new OrderingMethodFromQuery<TestOrderingProperty>
            {
                OrderBy = [],
                Asc = [],
            };
            var expected = OrderingMethod<TestOrderingProperty>.Default;

            // act
            var success = OrderingMethodFromQueryExtensions.TryConvertToOrderingMethod(source: source, out var result);

            // assert
            success.Should().Be(true);
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class OrderingMethodFromQueryExtensionsTests002 : OrderingMethodFromQueryExtensionsTests
    {
        [Fact]
        public void TryConvertToOrderingMethod_ValidInputMatchingNonZeroCounts_ReturnsTrueWithExpectedResult()
        {
            // arrange
            var source = new OrderingMethodFromQuery<TestOrderingProperty>
            {
                OrderBy = [TestOrderingProperty.Property1, TestOrderingProperty.Property2, TestOrderingProperty.Property3],
                Asc = [true, false, true],
            };
            var expected = new OrderingMethod<TestOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestOrderingProperty.Property1, Ascending = true },
                    new() { Property = TestOrderingProperty.Property2, Ascending = false },
                    new() { Property = TestOrderingProperty.Property3, Ascending = true },
                ],
            };

            // act
            var success = OrderingMethodFromQueryExtensions.TryConvertToOrderingMethod(source: source, out var result);

            // assert
            success.Should().Be(true);
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class OrderingMethodFromQueryExtensionsTests003 : OrderingMethodFromQueryExtensionsTests
    {
        [Fact]
        public void TryConvertToOrderingMethod_ValidInputOrderByHasMoreItemsThanAsc_ReturnsTrueWithExpectedResult()
        {
            // arrange
            var source = new OrderingMethodFromQuery<TestOrderingProperty>
            {
                OrderBy = [TestOrderingProperty.Property1, TestOrderingProperty.Property2, TestOrderingProperty.Property3],
                Asc = [false], // note: less items than OrderBy
            };
            var expected = new OrderingMethod<TestOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestOrderingProperty.Property1, Ascending = false },
                    // all other items have Ascending = true
                    new() { Property = TestOrderingProperty.Property2, Ascending = true },
                    new() { Property = TestOrderingProperty.Property3, Ascending = true },
                ],
            };

            // act
            var success = OrderingMethodFromQueryExtensions.TryConvertToOrderingMethod(source: source, out var result);

            // assert
            success.Should().Be(true);
            result.Should().BeEquivalentTo(expected);
        }
    }

    public class OrderingMethodFromQueryExtensionsTests004 : OrderingMethodFromQueryExtensionsTests
    {
        [Fact]
        public void TryConvertToOrderingMethod_InvalidInputOrderByHasLessItemsThanAsc_ReturnsFalse()
        {
            // arrange
            var source = new OrderingMethodFromQuery<TestOrderingProperty>
            {
                OrderBy = [TestOrderingProperty.Property1, TestOrderingProperty.Property2, TestOrderingProperty.Property3],
                Asc = [true, true, true, true], // note: more items than OrderBy
            };

            // act
            var success = OrderingMethodFromQueryExtensions.TryConvertToOrderingMethod(source: source, out var _);

            // assert
            success.Should().Be(false);
        }
    }

    private enum TestOrderingProperty
    {
        Property1,
        Property2,
        Property3,
    }
}
