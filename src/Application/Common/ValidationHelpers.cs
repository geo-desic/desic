using Desic.Application.Common.Exceptions;

namespace Desic.Application.Common;

internal static class ValidationHelpers
{
    internal static void GuardAgainstInvalid<T>(this FluentValidation.IValidator<T> validator, T instance)
    {
        var validationResult = validator.Validate(instance);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}
