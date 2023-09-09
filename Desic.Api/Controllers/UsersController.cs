using Desic.Entities;
using Desic.EntityFrameworkCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Desic.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
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
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IUser>> Get(long id)
        {
            using var loggerScope = _logger.BeginScope(new List<KeyValuePair<string, object>> { new KeyValuePair<string, object>("UserId", id) });
            _logger.LogInformation("UsersController.Get({id})", id);
            var result = await _context.Users.FirstOrDefaultAsync(x => x.SequentialId == id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}