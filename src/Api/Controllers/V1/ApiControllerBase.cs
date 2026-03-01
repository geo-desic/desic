using Desic.Application.Common;
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
}
