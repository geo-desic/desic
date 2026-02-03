using Desic.Entities;
using DesicTests.Desic;
using DesicWeb.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using System.Threading.Tasks;
using Xunit;

namespace DesicTests
{
    public class UsersControllerTests
    {
        [Fact]
        public async Task Get_WithInvalidId_ReturnsHttpNotFound()
        {
            var controller = new UsersController(new FakeUsersRepository(), new NullLogger<UsersController>());
            var result = (await controller.Get(0)).Result;
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Get_WithValidId_ReturnsNotNullUser()
        {
            var controller = new UsersController(new FakeUsersRepository(), new NullLogger<UsersController>());
            var result = (await controller.Get(1)).Result;
            var okResult = Assert.IsType<OkObjectResult>(result);
            var user = Assert.IsType<User>(okResult.Value);
            Assert.NotNull(user);
        }
    }
}
