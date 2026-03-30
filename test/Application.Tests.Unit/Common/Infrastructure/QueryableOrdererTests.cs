using AwesomeAssertions;
using Desic.Application.Common.Infrastructure;
using Desic.Shared.Extensions;
using System.Linq.Expressions;

namespace Desic.Application.Tests.Unit.Common.Infrastructure;

public class QueryableOrdererTests
{
    public class QueryableOrdererTests001 : QueryableOrdererTests
    {
        [Fact]
        public void OrderBy_AscendingTrue_ResultToListEquivalentToExpected()
        {
            // arrange
            Expression<Func<TestSource, DateTime?>> expectedKeySelector = s => s.NonUniqueDateTime;
            var orderer = new QueryableOrderer<TestSource, DateTime?> { KeySelector = expectedKeySelector };
            var query = GetItems().AsQueryable();
            var expected = query.OrderBy(keySelector: expectedKeySelector).ToList();

            // act
            var result = orderer.OrderBy(query: query, ascending: true);

            // assert
            var resultToList = result.ToList();
            resultToList.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableOrdererTests002 : QueryableOrdererTests
    {
        [Fact]
        public void OrderBy_AscendingFalse_ResultToListEquivalentToExpected()
        {
            // arrange
            Expression<Func<TestSource, DateTime?>> expectedKeySelector = s => s.NonUniqueDateTime;
            var orderer = new QueryableOrderer<TestSource, DateTime?> { KeySelector = expectedKeySelector };
            var query = GetItems().AsQueryable();
            var expected = query.OrderByDescending(keySelector: expectedKeySelector).ToList();

            // act
            var result = orderer.OrderBy(query: query, ascending: false);

            // assert
            var resultToList = result.ToList();
            resultToList.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableOrdererTests003 : QueryableOrdererTests
    {
        [Fact]
        public void ThenBy_AscendingTrue_ResultToListEquivalentToExpected()
        {
            // arrange
            Expression<Func<TestSource, DateTime?>> expectedKeySelector = s => s.NonUniqueDateTime;
            var orderer = new QueryableOrderer<TestSource, DateTime?> { KeySelector = expectedKeySelector };
            var query = GetItems().AsQueryable().OrderBy(x => x.NonUniqueString);
            var expected = query.ThenBy(keySelector: expectedKeySelector).ToList();

            // act
            var result = orderer.ThenBy(query: query, ascending: true);

            // assert
            var resultToList = result.ToList();
            resultToList.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class QueryableOrdererTests004 : QueryableOrdererTests
    {
        [Fact]
        public void ThenBy_AscendingFalse_ResultToListEquivalentToExpected()
        {
            // arrange
            Expression<Func<TestSource, DateTime?>> expectedKeySelector = s => s.NonUniqueDateTime;
            var orderer = new QueryableOrderer<TestSource, DateTime?> { KeySelector = expectedKeySelector };
            var query = GetItems().AsQueryable().OrderBy(x => x.NonUniqueString);
            var expected = query.ThenByDescending(keySelector: expectedKeySelector).ToList();

            // act
            var result = orderer.ThenBy(query: query, ascending: false);

            // assert
            var resultToList = result.ToList();
            resultToList.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
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

    private readonly static TestSource ItemU1NsANd2025 = new() { Unique = 1.ToGuid(), NonUniqueString = "A", NonUniqueDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = null, NonOrderableProperty = $"{nameof(TestSource.NonOrderableProperty)}Value1" };
    private readonly static TestSource ItemU2NsBNd2025 = new() { Unique = 2.ToGuid(), NonUniqueString = "B", NonUniqueDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = null }; // all orderable property values same as above except for Unique
    private readonly static TestSource ItemU3NsANd2025 = new() { Unique = 3.ToGuid(), NonUniqueString = "A", NonUniqueDateTime = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = null };
    private readonly static TestSource ItemU4NsBNd2020 = new() { Unique = 4.ToGuid(), NonUniqueString = "B", NonUniqueDateTime = new(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc), NonUniqueInteger = 1 };
    private readonly static TestSource ItemU5NsANdNull = new() { Unique = 5.ToGuid(), NonUniqueString = "A", NonUniqueDateTime = null, NonUniqueInteger = 1 };

    private class TestSource
    {
        public Guid Unique { get; set; }
        public required string NonUniqueString { get; set; }
        public DateTime? NonUniqueDateTime { get; set; }
        public int? NonUniqueInteger { get; set; }
        public string? NonOrderableProperty { get; set; }
    }
}
