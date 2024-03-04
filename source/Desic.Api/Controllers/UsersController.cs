using Desic.Api.Logging;
using Desic.EntityFrameworkCore.Entities;
using Desic.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desic.Api.Controllers
{
    [ApiController]
    [Route("v1/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly DesicContext _context;

        public UsersController(DesicContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<User>> Get([FromRoute] Guid id)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { new("UserId", id) });
            _logger.LogInformation(LogEvents.UsersGet, "UsersController.Get({id})", id);
            var result = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (result == null)
            {
                _logger.LogWarning(LogEvents.UsersGetNotFound, "UsersController.Get({id}) not found", id);
                return NotFound();
            }
            return Ok(result);
        }
    }
}