using Desic.Api.Logging;
using Desic.Business.Users;
using Desic.Business.Users.Models;
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> Get([FromRoute] Guid id)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { { new("controllerType", nameof(UsersController)) }, { new("userId", id) } });
            _logger.LogInformation(LogEvents.UsersGet, "Get({id})", id);

            var request = new GetUserByIdRequest { UserId = id };
            _logger.LogTrace("Invoking {requestType}: {@request}", nameof(GetUserByIdRequest), request);
            _logger.LogTrace("Invoking {requestType} with UserId {requestUserId}", nameof(GetUserByIdRequest), request.UserId);
            var result = await _mediator.Send(request);

            if (result == null)
            {
                _logger.LogWarning(LogEvents.UsersGetNotFound, "User with id {id} not found", id);
                return NotFound();
            }
            return Ok(result);
        }
    }
}