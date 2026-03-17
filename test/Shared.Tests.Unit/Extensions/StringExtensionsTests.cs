using AwesomeAssertions;
using Desic.Shared.Extensions;

namespace Desic.Shared.Tests.Unit.Extensions;

public class StringExtensionsTests
{
    public class StringExtensionsTests001 : StringExtensionsTests
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
