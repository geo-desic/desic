using AwesomeAssertions;
using Desic.Application.Users;

namespace Desic.Application.Tests.Unit.Users;

public class UserExtensionsTests
{
    public class UserExtensionsTests001 : UserExtensionsTests
    {
        [Fact]
        public void ToModel_AllMappedPropertiesSpecified_AllPropertiesMapped()
        {
            // arrange
            var expected = new User
            {
                Username = "username-expected",
            };
            var source = new Domain.Users.User
            {
                Username = expected.Username,
            };

            // act
            var result = UserExtensions.ToModel(source: source);

            // assert
            result.Should().BeEquivalentTo(expected);
        }
    }
}
