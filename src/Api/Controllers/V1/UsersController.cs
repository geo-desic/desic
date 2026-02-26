using Desic.Api.Dtos.Users;
using Desic.Api.Logging;
using Desic.Api.Mappings;
using Desic.Application.Common.Exceptions;
using Desic.Application.Users.Create;
using Desic.Application.Users.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
//[RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes")]
public class UsersController(ILogger<UsersController> logger, IMediator mediator) : ControllerBase
{
    private readonly ILogger<UsersController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<User>> Get([FromRoute] Guid id)
    {
        using var loggerScope2 = _logger.BeginScope("UserId:{userId}", id);

        _logger.LogInformation(LogEvents.UserGet, "Get({Id})", id);

        var request = new GetUserByIdRequest { UserId = id };
        var result = await _mediator.Send(request);

        return result.Match(onSuccess: u => Ok(u.ToDto()), onFailure: e => Problem(e), onNull: () => NotFound());
    }

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<User>> Create([FromBody] Dtos.Users.UserCreate user, [FromHeader(Name = "Prefer")] string? preferHeaderValue)
    {
        using var loggerScope2 = _logger.BeginScope("Username:{username}", user.Username);

        _logger.LogInformation(LogEvents.UserCreate, "Create(user, {PreferHeaderValue})", preferHeaderValue);

        var request = new CreateUserRequest { User = user.ToBusinessModel(), ReturnRepresentation = "return=representation".Equals(preferHeaderValue, StringComparison.OrdinalIgnoreCase) };
        var resultCreate = await _mediator.Send(request);
        if (resultCreate.IsFailure) return Problem(resultCreate.Exception);
        var value = resultCreate.Value;
        _logger.LogDebug(LogEvents.UserCreate, "Adding 'Entity-Id' response header with value {EntityId}", value.Id);
        HttpContext.Response.Headers.Append("Entity-Id", value.Id.ToString()); // always attach this header on success regardless of prefer header value
        if (value.Entity != null)
        {
            _logger.LogDebug(LogEvents.UserCreate, "Returning status code {StatusCode} and created user", StatusCodes.Status201Created);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, value.Entity.ToDto());
        }
        _logger.LogDebug(LogEvents.UserCreate, "Returning status code {StatusCode}", StatusCodes.Status204NoContent);
        return NoContent();
    }

    protected ActionResult Problem(Exception exception)
    {
        if (exception is ValidationException v)
        {
            var details = new ValidationProblemDetails(v.Errors);
            return ValidationProblem(details);
        }
        return Problem(statusCode: 400, detail: "One or more error occurred");
    }
}