using AwesomeAssertions;
using Desic.EntityFrameworkCore.Users.Queries;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Desic.EntityFrameworkCore.Tests.Unit.Users.Queries;

public class GetUserByUsernameRequestHandlerTests : AddUsersDependencyTests
{
    private readonly ILogger<GetUserByUsernameRequestHandler> _logger = NullLogger<GetUserByUsernameRequestHandler>.Instance;

    public class GetUserByUsernameRequestHandlerTests001 : GetUserByUsernameRequestHandlerTests
    {
        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNull()
        {
            // arrange
            AddUsers(1); // add a user to ensure sut doesn't just return null regardless of input
            var request = new GetUserByUsernameRequest { Username = "username_nonexistant" };
            var handler = new GetUserByUsernameRequestHandler(desicContext: _context, logger: _logger);

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.Should().BeNull();
        }
    }

    public class GetUserByUsernameRequestHandlerTests002 : GetUserByUsernameRequestHandlerTests
    {
        [Fact]
        public async Task Handle_UserExists_ReturnsExpectedUser()
        {
            // arrange
            var usersSeeded = AddUsers(3);
            var expected = usersSeeded[1]; // pick middle of 3 added users to ensure sut doesn't just return first or last user in the entire collection
            var request = new GetUserByUsernameRequest { Username = expected.Username };
            var handler = new GetUserByUsernameRequestHandler(desicContext: _context, logger: _logger);

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}