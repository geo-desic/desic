using AwesomeAssertions;
using Desic.Domain.Users;
using Desic.Infrastructure.Data.Handlers.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Desic.Infrastructure.Tests.Unit.Users.Commands;

public class CreateUserRequestHandlerTests : AddUsersDependencyTests
{
    private readonly ILogger<CreateUserRequestHandler> _logger = NullLogger<CreateUserRequestHandler>.Instance;

    public class CreateUserRequestHandlerTests001 : CreateUserRequestHandlerTests
    {
        [Fact]
        public async Task Handle_ValidNewUser_ExpectedResult()
        {
            // arrange
            var countBefore = 3;
            var idBefore = new Guid("A0000000-0000-0000-0000-000000000001"); // note: this userid starts with 'A' so it will not be equivalent to any users added
            AddUsers(countBefore);
            var userToBeCreated = NewUser(id: idBefore, username: "user_added");
            var request = new CreateUserRequest { User = userToBeCreated };
            var handler = new CreateUserRequestHandler(desicContext: _context, logger: _logger);

            // act
            var result = await handler.Handle(request, TestContext.Current.CancellationToken);
            var userCreated = await _context.Users.FirstOrDefaultAsync(x => x.Id == result, cancellationToken: TestContext.Current.CancellationToken);
            var countAfter = await _context.Users.CountAsync(cancellationToken: TestContext.Current.CancellationToken);

            // assert
            countAfter.Should().Be(countBefore + 1);
            result.Should().NotBeEmpty(); // Guid.Empty should never be assigned to a created user
            userCreated.Should().NotBeNull();
            userCreated.Id.Should().NotBe(idBefore); // a new id should be generated and assigned
            userCreated.Username.Should().Be(userToBeCreated.Username); // other fields are automatically set so do not assert on those
        }
    }
}
