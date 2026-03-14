using Desic.Api.Common;
using Desic.Application.Common;
using Desic.Application.Users;
using Desic.Application.Users.Create;
using Desic.Application.Users.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1;

[Route("v1/[controller]")]
public class UsersController(ILogger<UsersController> logger, IMediator mediator) : ApiControllerBase
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
        using var loggerScope = _logger.BeginScope("UserId:{userId}", id);
        _logger.LogInformation(LogEvents.UserGet, $"{nameof(UsersController)}.{nameof(Get)}({{{nameof(id)}}})", id);

        var request = new GetUserByIdRequest { Id = id };
        var result = await _mediator.Send(request);

        return result.Match(onSuccess: u => Ok(u), onFailure: e => Problem(e), onNull: () => NotFound());
    }

    [HttpPost]
    [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<User>> Create([FromBody] CreateUser user, [FromHeader(Name = Headers.Keys.Prefer)] string? preferHeaderValue)
    {
        using var loggerScope = _logger.BeginScope("Username:{username}", user.Username);
        _logger.LogInformation(LogEvents.UserCreate, $"{nameof(UsersController)}.{nameof(Create)}(user, {{{nameof(preferHeaderValue)}}})", preferHeaderValue);

        var request = new CreateUserRequest { Model = user, ReturnRepresentation = Headers.Values.IsPreferRepresentation(preferHeaderValue) };
        var resultCreate = await _mediator.Send(request);
        if (resultCreate.IsFailure) return Problem(resultCreate.Error);
        var value = resultCreate.Value;
        _logger.LogDebug(LogEvents.UserCreate, $"Adding '{Headers.Keys.EntityId}' response header with value {{{nameof(Headers.Keys.EntityId)}}}", value.Id);
        HttpContext.AddResponseHeaderEntityId(value.Id);
        if (value.Model != null)
        {
            return CreatedAtAction(nameof(Get), new { id = value.Id }, value.Model);
        }
        return NoContent();
    }
}