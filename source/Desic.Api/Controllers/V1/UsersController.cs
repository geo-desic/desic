using Desic.Api.Dtos.Users;
using Desic.Api.Logging;
using Desic.Api.Mappings;
using Desic.Api.Results;
using Desic.Business.Users;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desic.Api.Controllers.V1
{
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
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object?>> { { new("activityId", System.Diagnostics.Activity.Current?.Id) }, { new("endpointType", "UserGet") }, { new("userId", id) } });
            _logger.LogInformation(LogEvents.UsersGet, "Get({id})", id);

            var request = new GetUserByIdRequest { UserId = id };
            _logger.LogTrace("Invoking {requestType}: {@request}", nameof(GetUserByIdRequest), request);
            _logger.LogDebug("Invoking {requestType}", nameof(GetUserByIdRequest));
            var result = await _mediator.Send(request);

            if (result == null)
            {
                _logger.LogInformation(LogEvents.UsersGetNotFound, "User with id {id} not found", id);
                return NotFound();
            }
            return Ok(result.ToDto());
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<User>> Create([FromBody] UserCreate user, [FromHeader(Name = "Prefer")] string? preferHeaderValue)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object?>> { { new("activityId", System.Diagnostics.Activity.Current?.Id) }, { new("endpointType", "UserCreate") }, { new("username", user.Username) } });
            _logger.LogInformation(LogEvents.UsersCreate, "Create(user, {preferHeaderValue})", preferHeaderValue);
            _logger.LogTrace(LogEvents.UsersCreate, "{@user}", user);

            var request = new CreateUserRequest { User = user.ToBusinessModel(), ReturnResult = "return=representation".Equals(preferHeaderValue, StringComparison.OrdinalIgnoreCase) };
            _logger.LogTrace("Invoking {requestType}: {@request}", nameof(CreateUserRequest), request);
            _logger.LogDebug("Invoking {requestType}", nameof(CreateUserRequest));
            var resultCreate = await _mediator.Send(request);
            if (resultCreate.IsFailed)
            {
                return BadRequest(resultCreate.ToProblemDetails());
            }
            if (request.ReturnResult)
            {
                _logger.LogInformation(LogEvents.UsersCreate, "Returning status code {statusCode} and created user in response", StatusCodes.Status201Created);
                var userResult = resultCreate.Value.ToDto();
                return CreatedAtAction(nameof(Get), new { id = userResult.Id }, userResult);
            }
            return NoContent();
        }
    }
}