using FluentValidation;

namespace Desic.Application.Users.Create;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty()
            .Length(5, 50)
            .Matches("^[a-zA-Z0-9]+([._-]?[a-zA-Z0-9]+)*$"); // alphanumeric characters, hyphens '-', and periods '.'; no consecutive special characters, nor at start/end
    }
}
