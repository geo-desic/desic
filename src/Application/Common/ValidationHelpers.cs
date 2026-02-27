using Desic.Application.Common.Exceptions;

namespace Desic.Application.Common;

internal static class ValidationHelpers
{
    internal static ValidationException? ValidationException<T>(this FluentValidation.IValidator<T> validator, T instance)
    {
        var validationResult = validator.Validate(instance);
        if (!validationResult.IsValid)
        {
            return new ValidationException(validationResult.Errors);
        }
        return null;
    }
}
