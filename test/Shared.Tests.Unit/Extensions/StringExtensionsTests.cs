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

    public class StringExtensionsTests006 : StringExtensionsTests
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
        public void ChangeGuidCharacter_SourceGuidAsGuid_ExpectedResult(Guid expectedResult, string guid, char value, int characterIndex)
        {
            // act
            var result = guid.ChangeGuidCharacter(value: value, characterIndex: characterIndex);

            // assert
            result.Should().Be(expectedResult);
        }
    }

    public class StringExtensionsTests007 : StringExtensionsTests
    {
        [Theory]
        [InlineData(typeof(ArgumentOutOfRangeException), "0000000000000000000000000000000", '1', 0)]   // invalid guid length: 31
        [InlineData(typeof(ArgumentOutOfRangeException), "000000000000000000000000000000000", '1', 0)] // invalid guid length: 33
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", 47, 0)]   // invalid value
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", 58, 0)]   // invalid value
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", 64, 0)]   // invalid value
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", 71, 0)]   // invalid value
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", 96, 0)]   // invalid value
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", 103, 0)]  // invalid value
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", '1', -1)] // invalid characterIndex - negative
        [InlineData(typeof(ArgumentOutOfRangeException), "00000000000000000000000000000000", '1', 32)] // invalid characterIndex - larger than maximum character index
        public void ChangeGuidCharacter_SourceGuidAsGuid_ExpectedResult(Type expectedExceptionType, string guid, char value, int characterIndex)
        {
            // arrange
            var act = () => guid.ChangeGuidCharacter(value: value, characterIndex: characterIndex);

            // act / assert
            act.Should().Throw<Exception>().Which.Should().BeOfType(expectedExceptionType);
        }
    }
}
