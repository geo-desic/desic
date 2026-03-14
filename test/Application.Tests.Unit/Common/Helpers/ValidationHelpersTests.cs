using AwesomeAssertions;
using Desic.Application.Common.Helpers;
using FluentValidation;

namespace Desic.Application.Tests.Unit.Common.Helpers;

public class ValidationHelpersTests
{
    public class ValidationHelpersTests001 : ValidationHelpersTests
    {
        [Fact]
        public void InstanceIsValid_ValidModel_ReturnsTrueAndNullError()
        {
            // arrange
            var instance = new TestModel { Name = "Name" }; // valid - name is non-null
            var validator = new TestModelValidator();

            // act
            var result = ValidationHelpers.InstanceIsValid(validator: validator, instance: instance, out var error);

            // assert
            result.Should().BeTrue();
            error.Should().BeNull();
        }
    }

    public class ValidationHelpersTests002 : ValidationHelpersTests
    {
        [Fact]
        public void InstanceIsValid_InvalidModel_ReturnsFalseAndNonNullError()
        {
            // arrange
            var instance = new TestModel { Name = null }; // invalid - name is null
            var validator = new TestModelValidator();

            // act
            var result = ValidationHelpers.InstanceIsValid(validator: validator, instance: instance, out var error);

            // assert
            result.Should().BeFalse();
            error.Should().NotBeNull();
        }
    }

    private class TestModel
    {
        public string? Name { get; set; }
    }

    private class TestModelValidator : AbstractValidator<TestModel>
    {
        public TestModelValidator()
        {
            RuleFor(u => u.Name)
                .NotEmpty();
        }
    }
}
