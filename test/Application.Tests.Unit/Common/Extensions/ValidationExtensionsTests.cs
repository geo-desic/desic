using AwesomeAssertions;
using Desic.Application.Common.Extensions;
using FluentValidation;

namespace Desic.Application.Tests.Unit.Common.Extensions;

public class ValidationExtensionsTests
{
    public class ValidationExtensionsTests001 : ValidationExtensionsTests
    {
        [Fact]
        public void InstanceIsValid_ValidModel_ReturnsTrueAndNullError()
        {
            // arrange
            var instance = new TestModel { Name = "Name" }; // valid - name is non-null
            var validator = new TestModelValidator();

            // act
            var result = ValidationExtensions.InstanceIsValid(validator: validator, instance: instance, out var error);

            // assert
            result.Should().BeTrue();
            error.Should().BeNull();
        }
    }

    public class ValidationExtensionsTests002 : ValidationExtensionsTests
    {
        [Fact]
        public void InstanceIsValid_InvalidModel_ReturnsFalseAndNonNullError()
        {
            // arrange
            var instance = new TestModel { Name = null }; // invalid - name is null
            var validator = new TestModelValidator();

            // act
            var result = ValidationExtensions.InstanceIsValid(validator: validator, instance: instance, out var error);

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
