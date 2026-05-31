using AwesomeAssertions;
using Desic.Api.Controllers.V1;
using Desic.Application.Common;
using Desic.Application.Users;
using Desic.Application.Users.Get;
using DispatchR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Desic.Api.Tests.Unit.Controllers.V1;

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
            Setup(item: null);
            var controller = NewController();

            // act
            var result = (await controller.Get(_id, cancellationToken: TestContext.Current.CancellationToken))?.Result as NotFoundResult;

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
            var expectedItem = NewItem();
            Setup(item: expectedItem);
            var controller = NewController();

            // act
            var result = (await controller.Get(_id, cancellationToken: TestContext.Current.CancellationToken))?.Result as OkObjectResult;

            // assert
            result.Should().NotBeNull();
            result.StatusCode.Should().Be(200);
            result.Value.Should().BeEquivalentTo(expectedItem);
        }
    }

    private void Setup(User? item)
    {
        var result = item == null ? new Result<User>() : new Result<User>(item);
        _mediator.Setup(x => x.Send(It.IsAny<GetUserByIdRequest>(), It.IsAny<CancellationToken>())).ReturnsAsync(result);
    }

    private UsersController NewController()
    {
        return new UsersController(logger: _logger, mediator: _mediator.Object);
    }

    private static User NewItem(Guid? id = null)
    {
        id ??= Guid.CreateVersion7();
        return new User
        {
            Id = id.Value,
            Username = "username",
        };
    }
}
