using FluentResults;
using FluentValidation.Results;

namespace Desic.Application.Results;

internal static class ValidationHelpers
{
    internal static Result<T> ToFailResult<T>(this ValidationResult result)
    {
        return Result.Fail<T>(result.Errors.Select(e => new ValidationResultError(e.ErrorMessage, e.PropertyName, e.Severity.ToString())));
    }
}
