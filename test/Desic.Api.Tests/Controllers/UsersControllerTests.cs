using Desic.Api.Controllers.V1;
using Desic.Business.Users;
using Desic.Business.Users.Models;
using FluentAssertions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace Desic.Api.Tests.Controllers
{
    public class UsersControllerTests
    {
        private readonly Guid _id = new("00000000-0000-0000-0000-000000000001");
        private readonly Mock<IMediator> _mediator = new();

        [Fact]
        public async Task Get_UserDoesNotExist_Status404NotFound()
        {
            // arrange
            _mediator.Setup(x => x.Send(It.IsAny<GetUserByIdRequest>())).ReturnsAsync((User?)null);
            var controller = NewUsersController();

            // act
            var result = (await controller.Get(_id))?.Result as NotFoundResult;

            // assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Get_UserExistsButNotAuthorizedToAccess_Status403Forbidden()
        {
            // not implemented in controller yet
        }

        [Fact]
        public async Task Get_UserExistsAndAuthorizedToAccess_Status200OK()
        {
            // arrange
            var expectedUser = NewUser();
            _mediator.Setup(x => x.Send(It.IsAny<GetUserByIdRequest>())).ReturnsAsync(expectedUser);
            var controller = NewUsersController();

            // act
            var result = (await controller.Get(_id))?.Result as OkObjectResult;

            // assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(200);
            result?.Value.Should().BeSameAs(expectedUser);
        }

        private UsersController NewUsersController(ILogger<UsersController>? logger = null, Mock<IMediator>? mediator = null)
        {
            logger ??= NullLogger<UsersController>.Instance;
            mediator ??= _mediator;
            return new UsersController(logger: logger, mediator: mediator.Object);
        }

        private static User NewUser(Guid id = new())
        {
            return new User
            {
                Id = id,
                Username = "username",
            };
        }
    }
}
