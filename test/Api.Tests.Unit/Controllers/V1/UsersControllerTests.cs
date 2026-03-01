using AwesomeAssertions;
using Desic.Api.Controllers.V1;
using Desic.Application.Common;
using Desic.Application.Users;
using Desic.Application.Users.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Desic.Api.Tests.Unit.Controllers.V1;

// do not add and rely on any fixtures or similar to this parent class (inherited by the test classes below) that are not safe for parallel access
public class UsersControllerTests
{
    private readonly Guid _id = new("00000000-0000-0000-0000-000000000001");
    private readonly ILogger<UsersController> _logger = NullLogger<UsersController>.Instance;
    private readonly Mock<IMediator> _mediator = new();

    // each test has its own class so it can be run in parallel
    public class UserControllerTests001 : UsersControllerTests
    {
        [Fact]
        public async Task Get_UserDoesNotExist_Status404NotFound()
        {
            // arrange
            Setup(user: null);
            var controller = NewUsersController();

            // act
            var result = (await controller.Get(_id))?.Result as NotFoundResult;

            // assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(404);
        }
    }

    public class UserControllerTests002 : UsersControllerTests
    {
        [Fact]
        public async Task Get_UserExists_Status200OK()
        {
            // arrange
            var userExpected = NewUser();
            Setup(user: userExpected);
            var controller = NewUsersController();

            // act
            var result = (await controller.Get(_id))?.Result as OkObjectResult;

            // assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(userExpected);
        }
    }

    private void Setup(User? user)
    {
        var result = user == null ? new Result<User>() : new Result<User>(user);
        _mediator.Setup(x => x.Send(It.IsAny<GetUserByIdRequest>())).ReturnsAsync(result);
    }

    private UsersController NewUsersController()
    {
        return new UsersController(logger: _logger, mediator: _mediator.Object);
    }

    private static User NewUser(Guid? id = null)
    {
        id ??= Guid.CreateVersion7();
        return new User
        {
            Id = id.Value,
            Username = "username",
        };
    }
}
