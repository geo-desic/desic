using Desic.Business.Models.Users;
using FluentValidation;

namespace Desic.Business.Validators.Users;

public class UserCreateValidator : AbstractValidator<UserCreate>
{
    public UserCreateValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty()
            .Length(5, 50)
            .Matches("^[a-zA-Z0-9]+([._-]?[a-zA-Z0-9]+)*$"); // alphanumeric characters, hyphens '-', and periods '.'; no consecutive special characters, nor at start/end
    }
}
