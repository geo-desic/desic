using AwesomeAssertions;
using Desic.Shared.Extensions;

namespace Desic.Shared.Tests.Unit.Extensions;

public class StringExtensionsTests
{
    public class StringExtensionsTests001 : StringExtensionsTests
    {
        [Theory]
        [InlineData(null, null, 0)]
        [InlineData("", "", 0)]
        [InlineData("", "12345", 0)]
        [InlineData("1", "12345", 1)]
        [InlineData("12", "12345", 2)]
        [InlineData("123", "12345", 3)]
        [InlineData("1234", "12345", 4)]
        [InlineData("12345", "12345", 5)]
        [InlineData("12345", "12345", 6)]
        [InlineData("12345", "12345", 100)]
        public void Left_SpecifiedInputs_ExpectedResult(string? expectedResult, string? source, int length)
        {
            // act
            var result = source.Left(length: length);

            // assert
            result.Should().Be(expectedResult);
        }
    }

    public class StringExtensionsTests002 : StringExtensionsTests
    {
        [Fact]
        public void Left_NegativeLength_ThrowsExpectedException()
        {
            // arrage
            var act = () => "12345".Left(length: -1);

            // act / assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }

    public class StringExtensionsTests003 : StringExtensionsTests
    {
        [Theory]
        [InlineData(null, null, 0)]
        [InlineData("", "", 0)]
        [InlineData("", "12345", 0)]
        [InlineData("5", "12345", 1)]
        [InlineData("45", "12345", 2)]
        [InlineData("345", "12345", 3)]
        [InlineData("2345", "12345", 4)]
        [InlineData("12345", "12345", 5)]
        [InlineData("12345", "12345", 6)]
        [InlineData("12345", "12345", 100)]
        public void Right_SpecifiedInputs_ExpectedResult(string? expectedResult, string? source, int length)
        {
            // act
            var result = source.Right(length: length);

            // assert
            result.Should().Be(expectedResult);
        }
    }

    public class StringExtensionsTests004 : StringExtensionsTests
    {
        [Fact]
        public void Right_NegativeLength_ThrowsExpectedException()
        {
            // arrage
            var act = () => "12345".Right(length: -1);

            // act / assert
            act.Should().Throw<ArgumentOutOfRangeException>();
        }
    }

    public class StringExtensionsTests005 : StringExtensionsTests
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
        public void ToIntBasedGuid_SourceGuidAsString_ExpectedResult(Guid expectedResult, string guid, int value)
        {
            // act
            var result = guid.ToIntBasedGuid(value);

            // assert
            result.Should().Be(expectedResult);
        }
    }
}
