using AwesomeAssertions;
using Desic.Application.Common.Interfaces;
using Desic.Application.Users;
using Desic.Application.Users.Get;
using Desic.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Moq.EntityFrameworkCore;

namespace Desic.Application.Tests.Unit.Users.Get;

public class GetUserByIdRequestHandlerTests
{
    private readonly ILogger<GetUserByIdRequestHandler> _logger = NullLogger<GetUserByIdRequestHandler>.Instance;
    private readonly Mock<IApplicationDbContext> _dbContext = new();
    private readonly Guid IdThatExists = 1.ToGuid();
    private readonly Guid IdThatDoesNotExist = Guid.AllBitsSet;

    public class GetUserByIdRequestHandlerTests001 : GetUserByIdRequestHandlerTests
    {
        [Fact]
        public async Task Handle_RequestedUserExists_ReturnsExpectedUser()
        {
            // arrange
            Setup();
            var expected = DomainUser().ToDto();
            var handler = new GetUserByIdRequestHandler(logger: _logger, dbContext: _dbContext.Object);
            var request = new GetUserByIdRequest
            {
                UserId = IdThatExists,
            };

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.IsSuccess.Should().BeTrue();
            result.Value.Should().NotBeNull();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Should().BeEquivalentTo(expected);
        }
    }

    public class GetUserByIdRequestHandlerTests002 : GetUserByIdRequestHandlerTests
    {
        [Fact]
        public async Task Handle_RequestedUserDoesNotExist_ReturnsNull()
        {
            // arrange
            Setup();
            var handler = new GetUserByIdRequestHandler(logger: _logger, dbContext: _dbContext.Object);
            var request = new GetUserByIdRequest
            {
                UserId = IdThatDoesNotExist,
            };

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.IsNull.Should().BeTrue();
            result.Value.Should().BeNull();
        }
    }

    private Domain.Users.User DomainUser()
    {
        return new() { Id = IdThatExists, Username = "username" };
    }

    private void Setup(bool seedUser = true)
    {
        var users = new List<Domain.Users.User>();
        if (seedUser) users.Add(DomainUser());
        _dbContext.Setup(x => x.Users).ReturnsDbSet(users);
    }
}
