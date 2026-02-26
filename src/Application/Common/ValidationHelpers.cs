using FluentValidation;

namespace Desic.Application.Common;

internal static class ValidationHelpers
{
    internal static void GuardAgainstInvalid<T>(this IValidator<T> validator, T instance)
    {
        var validationResult = validator.Validate(instance);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }
    }
}
