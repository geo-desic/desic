using Desic.Api.Controllers.V1;
using Desic.Api.Dtos.Users;
using Desic.Business.Users;
using FluentAssertions;
using FluentResults;
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
            _mediator.Setup(x => x.Send(It.IsAny<GetUserByIdRequest>())).ReturnsAsync(Result.Fail($"User with id {_id} not found"));
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
            var userBusiness = NewUserBusiness();
            _mediator.Setup(x => x.Send(It.IsAny<GetUserByIdRequest>())).ReturnsAsync(Result.Ok(userBusiness));
            var controller = NewUsersController();
            var userExpected = ExpectedUserDto(userBusiness);

            // act
            var result = (await controller.Get(_id))?.Result as OkObjectResult;

            // assert
            result.Should().NotBeNull();
            result?.StatusCode.Should().Be(200);
            result?.Value.Should().BeEquivalentTo(userExpected);
        }

        private UsersController NewUsersController(ILogger<UsersController>? logger = null, Mock<IMediator>? mediator = null)
        {
            logger ??= NullLogger<UsersController>.Instance;
            mediator ??= _mediator;
            return new UsersController(logger: logger, mediator: mediator.Object);
        }

        private static User ExpectedUserDto(Business.Users.Models.User user)
        {
            return new User
            {
                Id = user.Id,
                Username = user.Username,
            };
        }

        private static Business.Users.Models.User NewUserBusiness(Guid id = new())
        {
            return new Business.Users.Models.User
            {
                Id = id,
                Username = "username",
            };
        }
    }
}
