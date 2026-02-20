using AwesomeAssertions;
using Desic.Helpers;

namespace Desic.Tests.Unit.Helpers;

public class NullableHelpersTests
{
    public class NullableHelpersTests001 : NullableHelpersTests
    {
        [Theory]
        [InlineData(true, null, null)]
        [InlineData(true, 1, 1)]
        [InlineData(true, 1, 2)]
        [InlineData(true, "", "")]
        [InlineData(true, "a", "b")]
        [InlineData(false, null, 1)]
        [InlineData(false, null, "")]
        [InlineData(false, 1, null)]
        [InlineData(false, "", null)]
        public void NullablyEquivalentTo_SpecifiedInput_ExpectedResult(bool expected, object? source, object? target)
        {
            // act
            var result = source.NullablyEquivalentTo(target);

            // assert
            expected.Should().Be(result);
        }
    }
}
