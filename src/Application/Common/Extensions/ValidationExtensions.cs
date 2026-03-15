namespace Desic.Application.Common.Extensions;

internal static class ValidationExtensions
{
    internal static bool InstanceIsValid<T>(this FluentValidation.IValidator<T> validator, T instance, out ValidationError? error)
    {
        error = null;
        var validationResult = validator.Validate(instance);
        var result = validationResult.IsValid;
        if (!result)
        {
            error = new ValidationError(validationResult.Errors);
        }
        return result;
    }
}
