using Desic.Application.Common.Models;
using Desic.Application.Common.Validators;
using FluentValidation.TestHelper;

namespace Desic.Application.Tests.Unit.Common.Validators;

public class OrderingMethodValidatorTests
{
    public class OrderingMethodValidatorTests001 : OrderingMethodValidatorTests
    {
        [Fact]
        public void ValidateOrderBy_ValidDataDefault_NoValidationError()
        {
            // arrange
            var model = OrderingMethod<TestOrderingProperty>.Default;
            var validator = new OrderingMethodValidator();

            // act
            var result = validator.TestValidate(model, o => o.IncludeProperties(m => m.OrderBy));

            // assert
            result.ShouldNotHaveValidationErrorFor(m => m.OrderBy);
        }
    }

    public class OrderingMethodValidatorTests002 : OrderingMethodValidatorTests
    {
        [Fact]
        public void ValidateOrderBy_ValidDataSpecified_NoValidationError()
        {
            // arrange
            var model = new OrderingMethod<TestOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestOrderingProperty.Property1 },
                    new() { Property = TestOrderingProperty.Property2 },
                    new() { Property = TestOrderingProperty.Property3 },
                ],
            };
            var validator = new OrderingMethodValidator();

            // act
            var result = validator.TestValidate(model, o => o.IncludeProperties(m => m.OrderBy));

            // assert
            result.ShouldNotHaveValidationErrorFor(m => m.OrderBy);
        }
    }

    public class OrderingMethodValidatorTests003 : OrderingMethodValidatorTests
    {
        [Fact]
        public void ValidateOrderBy_InvalidDataCollectionEmpty_ValidationError()
        {
            // arrange
            var model = new OrderingMethod<TestOrderingProperty> { OrderBy = [] };
            var validator = new OrderingMethodValidator();

            // act
            var result = validator.TestValidate(model, o => o.IncludeProperties(m => m.OrderBy));

            // assert
            result.ShouldHaveValidationErrorFor(m => m.OrderBy);
        }
    }

    public class OrderingMethodValidatorTests004 : OrderingMethodValidatorTests
    {
        [Fact]
        public void ValidateOrderBy_InvalidDataCollectionCountExceedsMaximum_ValidationError()
        {
            // arrange
            var model = new OrderingMethod<TestOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestOrderingProperty.Property1 },
                    new() { Property = TestOrderingProperty.Property2 },
                    new() { Property = TestOrderingProperty.Property3 },
                    new() { Property = TestOrderingProperty.Property4 }, // exceeds maximum count of 3
                ],
            };
            var validator = new OrderingMethodValidator();

            // act
            var result = validator.TestValidate(model, o => o.IncludeProperties(m => m.OrderBy));

            // assert
            result.ShouldHaveValidationErrorFor(m => m.OrderBy);
        }
    }

    public class OrderingMethodValidatorTests005 : OrderingMethodValidatorTests
    {
        [Fact]
        public void ValidateOrderBy_InvalidDataCollectionHasDuplicatePropertyValues_ValidationError()
        {
            // arrange
            var model = new OrderingMethod<TestOrderingProperty>
            {
                OrderBy =
                [
                    new() { Property = TestOrderingProperty.Property1 },
                    new() { Property = TestOrderingProperty.Property1 }, // duplicate property value
                ],
            };
            var validator = new OrderingMethodValidator();

            // act
            var result = validator.TestValidate(model, o => o.IncludeProperties(m => m.OrderBy));

            // assert
            result.ShouldHaveValidationErrorFor(m => m.OrderBy);
        }
    }

    private enum TestOrderingProperty
    {
        Property1,
        Property2,
        Property3,
        Property4,
    }
}
