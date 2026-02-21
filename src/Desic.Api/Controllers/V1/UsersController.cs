using Desic.Api.Dtos.Users;
using Desic.Api.Logging;
using Desic.Api.Mappings;
using Desic.Business.Requests.Commands.Users;
using Desic.Business.Requests.Queries.Users;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[ApiController]
[Route("v1/[controller]")]
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

        if (result.IsFailed)
        {
            _logger.LogInformation(LogEvents.UserGet, "User with id {Id} not found", id);
            return NotFound();
        }
        return Ok(result.Value.ToDto());
    }

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<User>> Create([FromBody] UserCreate user, [FromHeader(Name = "Prefer")] string? preferHeaderValue)
    {
        using var loggerScope2 = _logger.BeginScope("Username:{username}", user.Username);

        _logger.LogInformation(LogEvents.UserCreate, "Create(user, {PreferHeaderValue})", preferHeaderValue);

        var request = new CreateUserRequest { User = user.ToBusinessModel(), ReturnResult = "return=representation".Equals(preferHeaderValue, StringComparison.OrdinalIgnoreCase) };
        var resultCreate = await _mediator.Send(request);
        if (resultCreate.IsFailed)
        {
            return Problem(resultCreate);
        }
        var userBusiness = resultCreate.Value;
        _logger.LogDebug(LogEvents.UserCreate, "Adding 'Entity-Id' response header with value {EntityId}", userBusiness.Id);
        HttpContext.Response.Headers.Append("Entity-Id", userBusiness.Id.ToString()); // always attach this header on success regardless of prefer header value
        if (request.ReturnResult)
        {
            _logger.LogDebug(LogEvents.UserCreate, "Returning status code {StatusCode} and created user", StatusCodes.Status201Created);
            var userResult = userBusiness.ToDto();
            return CreatedAtAction(nameof(Get), new { id = userResult.Id }, userResult);
        }
        _logger.LogDebug(LogEvents.UserCreate, "Returning status code {StatusCode}", StatusCodes.Status204NoContent);
        return NoContent();
    }

    protected ObjectResult Problem<T>(Result<T> result)
    {
        return Problem(statusCode: 400, detail: "One or more error occurred", extensions: new Dictionary<string, object?> { ["errors"] = result.Errors.Select(e => e.Message) });
    }
}