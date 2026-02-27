namespace Desic.Application.Common;

internal static class ValidationHelpers
{
    internal static ValidationError? ValidationError<T>(this FluentValidation.IValidator<T> validator, T instance)
    {
        var validationResult = validator.Validate(instance);
        if (!validationResult.IsValid)
        {
            return new ValidationError(validationResult.Errors);
        }
        return null;
    }
}
