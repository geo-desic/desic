using AwesomeAssertions;
using Desic.Core.Helpers;

namespace Desic.Core.Tests.Unit.Helpers;

public class GuidHelpersTests
{
    public class GuidHelpersTests001 : GuidHelpersTests
    {
        [Theory]
        [InlineData("00000000-0000-0000-0000-000000000001", -1)]
        [InlineData("00000000-0000-0000-0000-000000000001", 1)]
        [InlineData("00000000-0000-0000-0000-000000000010", 10)]
        [InlineData("00000000-0000-0000-0000-000000000100", 100)]
        [InlineData("00000000-0000-0000-0000-000000001000", 1000)]
        [InlineData("00000000-0000-0000-0000-000000010000", 10000)]
        [InlineData("00000000-0000-0000-0000-000000100000", 100000)]
        [InlineData("00000000-0000-0000-0000-000001000000", 1000000)]
        [InlineData("00000000-0000-0000-0000-000010000000", 10000000)]
        [InlineData("00000000-0000-0000-0000-000100000000", 100000000)]
        [InlineData("00000000-0000-0000-0000-001000000000", 1000000000)]
        public void ToGuid_VariousInputs_ExpectedResult(Guid expectedResult, int value)
        {
            // act
            var result = value.ToGuid();

            // assert
            result.Should().Be(expectedResult);
        }
    }

    public class GuidHelpersTests002 : GuidHelpersTests
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

    public class GuidHelpersTests003 : GuidHelpersTests
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
