using AwesomeAssertions;
using Desic.EntityFrameworkCore.Users.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Desic.EntityFrameworkCore.Tests.Unit.Users.Queries;

public class GetUserByIdRequestHandlerTests : AddUsersDependencyTests
{
    private readonly ILogger<GetUserByIdRequestHandler> _logger = NullLogger<GetUserByIdRequestHandler>.Instance;

    public class GetUserByIdRequestHandlerTests001 : GetUserByIdRequestHandlerTests
    {
        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNull()
        {
            // arrange
            AddUsers(1); // add a user to ensure sut doesn't just return null regardless of input
            var request = new GetUserByIdRequest { UserId = new("A0000000-0000-0000-0000-000000000001") }; // note: this userid starts with 'A' to ensure it will not match any added users
            var handler = new GetUserByIdRequestHandler(desicContext: _context, logger: _logger);

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.Should().BeNull();
        }
    }

    public class GetUserByIdRequestHandlerTests002 : GetUserByIdRequestHandlerTests
    {
        [Fact]
        public async Task Handle_UserExists_ReturnsExpectedUser()
        {
            // arrange
            var usersSeeded = AddUsers(3);
            var expected = usersSeeded[1]; // pick middle of 3 added users to ensure sut doesn't just return first or last user in the entire collection
            var request = new GetUserByIdRequest { UserId = expected.Id };
            var handler = new GetUserByIdRequestHandler(desicContext: _context, logger: _logger);

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
