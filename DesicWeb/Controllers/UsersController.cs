using System.Threading.Tasks;
using Desic.Entities;
using Desic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DesicWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUsersRepository _repository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUsersRepository repository, ILogger<UsersController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IUser>> Get(long id)
        {
            _logger.LogInformation($"UsersController.Get({id})");
            var result = await _repository.Get(id);
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
