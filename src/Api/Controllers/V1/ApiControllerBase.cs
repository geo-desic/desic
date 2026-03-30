using Desic.Api.Common.Extensions;
using Desic.Api.Dtos;
using Desic.Application.Common;
using Desic.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class ApiControllerBase : ControllerBase
{
    protected ActionResult Problem(Error error)
    {
        if (error is ValidationError v)
        {
            var details = new ValidationProblemDetails(v.Failures);
            return ValidationProblem(details);
        }
        return Problem(statusCode: 400, detail: error.Message);
    }

    protected OrderingMethod<T>? ConvertOrderingMethod<T>(OrderingMethodFromQuery<T> orderingMethodFromQuery, out ObjectResult? problemResult, ILogger? logger = null, EventId logEventId = default) where T : struct, Enum
    {
        problemResult = null;
        if (!orderingMethodFromQuery.TryConvertToOrderingMethod(out var result))
        {
            const string message = $"{nameof(OrderingMethodFromQuery<>)} is invalid";
            logger?.LogInformation(logEventId, message);
            problemResult = Problem(statusCode: 400, detail: message);
            return null;
        }
        return result;
    }
}
