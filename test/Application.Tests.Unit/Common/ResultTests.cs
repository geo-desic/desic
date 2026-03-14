using AwesomeAssertions;
using Desic.Application.Common;

namespace Desic.Application.Tests.Unit.Common;

public class ResultTests
{
    public class ResultTests001 : ResultTests
    {
        [Fact]
        public void NewResult_WithNullValue_IsNullAndValueIsNull()
        {
            // act
            var result = new Result<TestModel>();

            // assert
            result.Value.Should().BeNull();
            result.Error.Should().BeNull();
            result.IsNull.Should().BeTrue();
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeFalse();
        }
    }

    public class ResultTests002 : ResultTests
    {
        [Fact]
        public void NewResult_WithNonNullValue_IsSuccessAndValueIsExpected()
        {
            // arrange
            var expectedValue = new TestModel();

            // act
            var result = new Result<TestModel>(expectedValue);

            // assert
            result.Value.Should().Be(expectedValue);
            result.Error.Should().BeNull();
            result.IsNull.Should().BeFalse();
            result.IsSuccess.Should().BeTrue();
            result.IsFailure.Should().BeFalse();
        }
    }

    public class ResultTests003 : ResultTests
    {
        [Fact]
        public void NewResult_WithError_IsFailureAndErrorIsExpected()
        {
            // arrange
            var error = new Error(string.Empty);

            // act
            var result = new Result<TestModel>(error);

            // assert
            result.Value.Should().BeNull();
            result.Error.Should().Be(error);
            result.IsNull.Should().BeFalse();
            result.IsSuccess.Should().BeFalse();
            result.IsFailure.Should().BeTrue();
        }
    }

    public class ResultTests004 : ResultTests
    {
        [Fact]
        public void Match_WithNullValueAndNoOnNullFunctionProvided_ThrowsExpectedException()
        {
            // arrange
            var result = new Result<TestModel>();
            var match = () => result.Match(onSuccess: x => true, onFailure: e => false);

            // act/assert
            match.Should().Throw<InvalidOperationException>().WithMessage(Result<TestModel>.ExceptionMessageNullResultWithoutOnNullFunction);
        }
    }

    public class ResultTests005 : ResultTests
    {
        [Fact]
        public void Match_WithNullValueAndOnNullFunctionProvided_OnNullFunctionResultReturned()
        {
            // arrange
            var result = new Result<TestModel>();

            // act
            var matchResult = result.Match(onSuccess: x => false, onFailure: e => false, onNull: () => true); // only onNull function returns true

            // assert
            matchResult.Should().BeTrue();
        }
    }

    public class ResultTests006 : ResultTests
    {
        [Fact]
        public void Match_WithNonNullValue_OnSuccessFunctionResultReturned()
        {
            // arrange
            var result = new Result<TestModel>(new TestModel());

            // act
            var matchResult = result.Match(onSuccess: x => true, onFailure: e => false, onNull: () => false); // only onSuccess function returns true

            // assert
            matchResult.Should().BeTrue();
        }
    }

    public class ResultTests007 : ResultTests
    {
        [Fact]
        public void Match_WithError_OnFailureFunctionResultReturned()
        {
            // arrange
            var result = new Result<TestModel>(new Error(string.Empty));

            // act
            var matchResult = result.Match(onSuccess: x => false, onFailure: e => true, onNull: () => false); // only onFailure function returns true

            // assert
            matchResult.Should().BeTrue();
        }
    }

    private class TestModel { }
}
