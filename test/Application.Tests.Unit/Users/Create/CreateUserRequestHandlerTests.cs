using AwesomeAssertions;
using Desic.Application.Common.Interfaces;
using Desic.Application.Users.Create;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit.Users.Create;

public class CreateUserRequestHandlerTests
{
    private readonly ILogger<CreateUserRequestHandler> _logger = NullLogger<CreateUserRequestHandler>.Instance;
    private readonly Mock<IApplicationDbContext> _dbContext = new();
    private readonly IValidator<UserCreate> _validator = new UserCreateValidator();

    public class CreateUserRequestHandlerTests001 : CreateUserRequestHandlerTests
    {
        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task Handle_ValidRequestReturnResultFalse_SuccessResultWithCreatedUser(bool returnResult)
        {
            // arrange
            Setup();
            var handler = new CreateUserRequestHandler(logger: _logger, dbContext: _dbContext.Object, validator: _validator);
            var request = new CreateUserRequest
            {
                User = new UserCreate { Username = "username" },
                ReturnRepresentation = returnResult,
            };

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.IsSuccess.Should().BeTrue();
            _dbContext.Verify(x => x.Users.Add(It.IsAny<Domain.Users.User>()), Times.Once());
            _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once());
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBeEmpty();
            if (returnResult)
            {
                result.Value.Model.Should().NotBeNull();
                result.Value.Model.Username.Should().Be(request.User.Username);
            }
        }
    }

    public class CreateUserRequestHandlerTests002 : CreateUserRequestHandlerTests
    {
        [Fact]
        public async Task Handle_InvalidUsername_FailureResult()
        {
            // arrange
            Setup();
            var handler = new CreateUserRequestHandler(logger: _logger, dbContext: _dbContext.Object, validator: _validator);
            var request = new CreateUserRequest
            {
                User = new UserCreate { Username = "invalid username" }, // contains space character
            };

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.IsSuccess.Should().BeFalse();
            _dbContext.Verify(x => x.Users.Add(It.IsAny<Domain.Users.User>()), Times.Never());
            _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }

    public class CreateUserRequestHandlerTests003 : CreateUserRequestHandlerTests
    {
        [Fact]
        public async Task Handle_UsernameExists_FailureResult()
        {
            // arrange
            var username = "username";
            Setup(usernameSeed: username);
            var handler = new CreateUserRequestHandler(logger: _logger, dbContext: _dbContext.Object, validator: _validator);
            var request = new CreateUserRequest
            {
                User = new UserCreate { Username = username },
            };

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.IsSuccess.Should().BeFalse();
            _dbContext.Verify(x => x.Users.Add(It.IsAny<Domain.Users.User>()), Times.Never());
            _dbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never());
        }
    }

    private void Setup(string? usernameSeed = null)
    {
        var users = new List<Domain.Users.User>();
        if (usernameSeed != null) users.Add(new Domain.Users.User { Username = usernameSeed });
        _dbContext.Setup(x => x.Users).ReturnsDbSet(users);
    }
}
