using Desic.Application.Users.Create;
using FluentValidation.TestHelper;
using System.Text;

namespace Desic.Application.Tests.Unit.Users.Create;

public class CreateUserValidatorTests
{
    private readonly CreateUserValidator _validator = new();

    public class UserCreateValidatorTests001 : CreateUserValidatorTests
    {
        [Theory]
        [InlineData("username.01_is-valid")]
        [InlineData("abcdefghijklmnopqrstuvwxyz.0123_456-789")]
        public void ValidateUsername_ValidData_NoValidationError(string username)
        {
            // arrange
            var model = new CreateUser { Username = username };

            // act
            var result = _validator.TestValidate(model, o => o.IncludeProperties(m => m.Username));

            // assert
            result.ShouldNotHaveValidationErrorFor(m => m.Username);
        }
    }

    public class UserCreateValidatorTests002 : CreateUserValidatorTests
    {
        [Theory]
        [InlineData("aaaa")] // too short (4 characters)
        [InlineData("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa")] // too long (51 characters)
        [InlineData(".username")] // starts with non alphanumeric character
        [InlineData("_username")] // starts with non alphanumeric character
        [InlineData("-username")] // starts with non alphanumeric character
        [InlineData("username.")] // ends with non alphanumeric character
        [InlineData("username_")] // ends with non alphanumeric character
        [InlineData("username-")] // ends with non alphanumeric character
        public void ValidateUsername_InvalidData_ValidationError(string username)
        {
            // arrange
            var model = new CreateUser { Username = username };

            // act
            var result = _validator.TestValidate(model, o => o.IncludeProperties(m => m.Username));

            // assert
            result.ShouldHaveValidationErrorFor(m => m.Username);
        }
    }

    public class UserCreateValidatorTests003 : CreateUserValidatorTests
    {
        [Fact]
        public void ValidateUsername_InvalidData_ValidationError()
        {
            // arrange
            var sb = new StringBuilder();
            for (var c = 0; c < 32; ++c) // non-printable characers (ascii 0 through 31)
            {
                sb.Append((char)c);
            }
            sb.Append(" !\"#$%&'()*+,/:;<=>?@[\\]^`{|}~"); // other invalid ascii characters
            for (var c = 127; c < 256; ++c) // [del] and extended ascii characters
            {
                sb.Append((char)c);
            }
            var invalidCharacters = sb.ToString();

            foreach (var c in invalidCharacters)
            {
                var model = new CreateUser { Username = $"user{c}name" };

                // act
                var result = _validator.TestValidate(model, o => o.IncludeProperties(m => m.Username));

                // assert
                result.ShouldHaveValidationErrorFor(m => m.Username);
            }
        }
    }
}
