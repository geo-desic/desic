using AwesomeAssertions;
using Desic.Shared.Extensions;

namespace Desic.Shared.Tests.Unit.Extensions;

public class IntExtensionsTests
{
    public class IntExtensionsTests001 : IntExtensionsTests
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
}
