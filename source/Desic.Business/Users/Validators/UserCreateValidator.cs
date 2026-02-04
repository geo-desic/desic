using Desic.Business.Users.Models;
using FluentValidation;

namespace Desic.Business.Users.Validators
{
    public class UserCreateValidator : AbstractValidator<UserCreate>
    {
        public UserCreateValidator()
        {
            RuleFor(u => u.Username).NotEmpty().Length(5, 50).Matches("^[a-zA-Z0-9_]+$");
        }
    }
}
