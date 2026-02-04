using FluentResults;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Results
{
    public static class ResultHelpers
    {
        public static ProblemDetails ToProblemDetails(this Result result, int statusCode = StatusCodes.Status400BadRequest)
        {
            return new ProblemDetails
            {
                Title = "One or more error occurred",
                Status = statusCode,
                Detail = string.Join("; ", result.Errors.Select(e => e.Message))
            };
        }

        public static ProblemDetails ToProblemDetails<T>(this Result<T> result, int statusCode = StatusCodes.Status400BadRequest)
        {
            return new ProblemDetails
            {
                Title = "One or more error occurred",
                Status = statusCode,
                Detail = string.Join("; ", result.Errors.Select(e => e.Message))
            };
        }
    }
}
