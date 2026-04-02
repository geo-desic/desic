using AwesomeAssertions;
using Desic.Shared.Extensions;
using System.Data.SqlTypes;

namespace Desic.Shared.Tests.Unit.Extensions;

public class GuidExtensionsTests
{
    private const int GuidGenerationCount = 25;
    private const int GuidGenerationDelayMs = 2;

    public class GuidExtensionsTests001 : GuidExtensionsTests
    {
        [Theory]
        [InlineData("00000000-0000-0000-0000-ABCDEABCDEA1", "00000000-0000-0000-0000-ABCDEABCDEAB", -1)]
        [InlineData("00000000-0000-0000-0000-ABCDEABCDEA1", "00000000-0000-0000-0000-ABCDEABCDEAB", 1)]
        [InlineData("00000000-0000-0000-0000-ABCDEABCDE10", "00000000-0000-0000-0000-ABCDEABCDEAB", 10)]
        [InlineData("00000000-0000-0000-0000-ABCDEABCD100", "00000000-0000-0000-0000-ABCDEABCDEAB", 100)]
        [InlineData("00000000-0000-0000-0000-ABCDEABC1000", "00000000-0000-0000-0000-ABCDEABCDEAB", 1000)]
        [InlineData("00000000-0000-0000-0000-ABCDEAB10000", "00000000-0000-0000-0000-ABCDEABCDEAB", 10000)]
        [InlineData("00000000-0000-0000-0000-ABCDEA100000", "00000000-0000-0000-0000-ABCDEABCDEAB", 100000)]
        [InlineData("00000000-0000-0000-0000-ABCDE1000000", "00000000-0000-0000-0000-ABCDEABCDEAB", 1000000)]
        [InlineData("00000000-0000-0000-0000-ABCD10000000", "00000000-0000-0000-0000-ABCDEABCDEAB", 10000000)]
        [InlineData("00000000-0000-0000-0000-ABC100000000", "00000000-0000-0000-0000-ABCDEABCDEAB", 100000000)]
        [InlineData("00000000-0000-0000-0000-AB1000000000", "00000000-0000-0000-0000-ABCDEABCDEAB", 1000000000)]
        public void ToIntBasedGuid_SourceGuidAsGuid_ExpectedResult(Guid expectedResult, Guid guid, int value)
        {
            // act
            var result = guid.ToIntBasedGuid(value);

            // assert
            result.Should().Be(expectedResult);
        }
    }

    public class GuidExtensionsTests002 : GuidExtensionsTests
    {
        [Theory]
        [InlineData("10000000000000000000000000000000", "00000000000000000000000000000000", '1', 0)]
        [InlineData("01000000000000000000000000000000", "00000000000000000000000000000000", '1', 1)]
        [InlineData("00100000000000000000000000000000", "00000000000000000000000000000000", '1', 2)]
        [InlineData("00010000000000000000000000000000", "00000000000000000000000000000000", '1', 3)]
        [InlineData("00001000000000000000000000000000", "00000000000000000000000000000000", '1', 4)]
        [InlineData("00000100000000000000000000000000", "00000000000000000000000000000000", '1', 5)]
        [InlineData("00000010000000000000000000000000", "00000000000000000000000000000000", '1', 6)]
        [InlineData("00000001000000000000000000000000", "00000000000000000000000000000000", '1', 7)]
        [InlineData("00000000100000000000000000000000", "00000000000000000000000000000000", '1', 8)]
        [InlineData("00000000010000000000000000000000", "00000000000000000000000000000000", '1', 9)]
        [InlineData("00000000001000000000000000000000", "00000000000000000000000000000000", '1', 10)]
        [InlineData("00000000000100000000000000000000", "00000000000000000000000000000000", '1', 11)]
        [InlineData("00000000000010000000000000000000", "00000000000000000000000000000000", '1', 12)]
        [InlineData("00000000000001000000000000000000", "00000000000000000000000000000000", '1', 13)]
        [InlineData("00000000000000100000000000000000", "00000000000000000000000000000000", '1', 14)]
        [InlineData("00000000000000010000000000000000", "00000000000000000000000000000000", '1', 15)]
        [InlineData("00000000000000001000000000000000", "00000000000000000000000000000000", '1', 16)]
        [InlineData("00000000000000000100000000000000", "00000000000000000000000000000000", '1', 17)]
        [InlineData("00000000000000000010000000000000", "00000000000000000000000000000000", '1', 18)]
        [InlineData("00000000000000000001000000000000", "00000000000000000000000000000000", '1', 19)]
        [InlineData("00000000000000000000100000000000", "00000000000000000000000000000000", '1', 20)]
        [InlineData("00000000000000000000010000000000", "00000000000000000000000000000000", '1', 21)]
        [InlineData("00000000000000000000001000000000", "00000000000000000000000000000000", '1', 22)]
        [InlineData("00000000000000000000000100000000", "00000000000000000000000000000000", '1', 23)]
        [InlineData("00000000000000000000000010000000", "00000000000000000000000000000000", '1', 24)]
        [InlineData("00000000000000000000000001000000", "00000000000000000000000000000000", '1', 25)]
        [InlineData("00000000000000000000000000100000", "00000000000000000000000000000000", '1', 26)]
        [InlineData("00000000000000000000000000010000", "00000000000000000000000000000000", '1', 27)]
        [InlineData("00000000000000000000000000001000", "00000000000000000000000000000000", '1', 28)]
        [InlineData("00000000000000000000000000000100", "00000000000000000000000000000000", '1', 29)]
        [InlineData("00000000000000000000000000000010", "00000000000000000000000000000000", '1', 30)]
        [InlineData("00000000000000000000000000000001", "00000000000000000000000000000000", '1', 31)]
        [InlineData("01111111111111111111111111111111", "11111111111111111111111111111111", '0', 0)]
        [InlineData("20000000000000000000000000000000", "00000000000000000000000000000000", '2', 0)]
        [InlineData("30000000000000000000000000000000", "00000000000000000000000000000000", '3', 0)]
        [InlineData("40000000000000000000000000000000", "00000000000000000000000000000000", '4', 0)]
        [InlineData("50000000000000000000000000000000", "00000000000000000000000000000000", '5', 0)]
        [InlineData("60000000000000000000000000000000", "00000000000000000000000000000000", '6', 0)]
        [InlineData("70000000000000000000000000000000", "00000000000000000000000000000000", '7', 0)]
        [InlineData("80000000000000000000000000000000", "00000000000000000000000000000000", '8', 0)]
        [InlineData("90000000000000000000000000000000", "00000000000000000000000000000000", '9', 0)]
        [InlineData("A0000000000000000000000000000000", "00000000000000000000000000000000", 'a', 0)]
        [InlineData("A0000000000000000000000000000000", "00000000000000000000000000000000", 'A', 0)]
        [InlineData("B0000000000000000000000000000000", "00000000000000000000000000000000", 'b', 0)]
        [InlineData("B0000000000000000000000000000000", "00000000000000000000000000000000", 'B', 0)]
        [InlineData("C0000000000000000000000000000000", "00000000000000000000000000000000", 'c', 0)]
        [InlineData("C0000000000000000000000000000000", "00000000000000000000000000000000", 'C', 0)]
        [InlineData("D0000000000000000000000000000000", "00000000000000000000000000000000", 'd', 0)]
        [InlineData("D0000000000000000000000000000000", "00000000000000000000000000000000", 'D', 0)]
        [InlineData("E0000000000000000000000000000000", "00000000000000000000000000000000", 'e', 0)]
        [InlineData("E0000000000000000000000000000000", "00000000000000000000000000000000", 'E', 0)]
        [InlineData("F0000000000000000000000000000000", "00000000000000000000000000000000", 'f', 0)]
        [InlineData("F0000000000000000000000000000000", "00000000000000000000000000000000", 'F', 0)]
        public void ChangeGuidCharacter_SourceGuidAsGuid_ExpectedResult(Guid expectedResult, Guid guid, char value, int characterIndex)
        {
            // act
            var result = guid.ChangeGuidCharacter(value: value, characterIndex: characterIndex);

            // assert
            result.Should().Be(expectedResult);
        }
    }

    public class GuidExtensionsTests003 : GuidExtensionsTests
    {
        [Fact]
        public async Task CreateSequentialGuid_SpecifiedCountWithForSqlServerFalse_GeneratedGuidsSortSequentially()
        {
            // arrange
            var expected = new List<GuidAndSequentialId>();

            // act
            for (var i = 0; i < GuidGenerationCount; ++i)
            {
                expected.Add(new() { Guid = Guid.CreateSequentialGuid(forSqlServer: false), SequentialId = i });
                await Task.Delay(GuidGenerationDelayMs, cancellationToken: TestContext.Current.CancellationToken);
            }

            // assert
            var result = expected.OrderBy(x => x.Guid).ToList();
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class GuidExtensionsTests004 : GuidExtensionsTests
    {
        [Fact]
        public async Task CreateSequentialGuid_SpecifiedCountWithForSqlServerTrue_GeneratedGuidsSortSequentially()
        {
            // arrange
            var expected = new List<SqlGuidAndSequentialId>();

            // act
            for (var i = 0; i < GuidGenerationCount; ++i)
            {
                expected.Add(new() { Guid = Guid.CreateSequentialGuid(forSqlServer: true), SequentialId = i });
                await Task.Delay(GuidGenerationDelayMs, cancellationToken: TestContext.Current.CancellationToken);
            }

            // assert
            var result = expected.OrderBy(x => x.Guid).ToList();
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    public class GuidExtensionsTests005 : GuidExtensionsTests
    {
        [Fact]
        public async Task CreateSequentialGuidForSqlServer_SpecifiedCount_GeneratedGuidsSortSequentially()
        {
            // arrange
            var expected = new List<SqlGuidAndSequentialId>();

            // act
            for (var i = 0; i < GuidGenerationCount; ++i)
            {
                expected.Add(new() { Guid = Guid.CreateSequentialGuidForSqlServer(), SequentialId = i });
                await Task.Delay(GuidGenerationDelayMs, cancellationToken: TestContext.Current.CancellationToken);
            }

            // assert
            var result = expected.OrderBy(x => x.Guid).ToList();
            result.Should().BeEquivalentTo(expected, opt => opt.WithStrictOrdering());
        }
    }

    private class GuidAndSequentialId
    {
        public Guid Guid { get; set; }
        public int SequentialId { get; set; }
    }

    private class SqlGuidAndSequentialId
    {
        public SqlGuid Guid { get; set; } // the SqlGuid data type correctly implements Sql Server's unqueidentifier sorting
        public int SequentialId { get; set; }
    }
}
