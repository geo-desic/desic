using FluentValidation;

namespace Desic.Application.Users.Create;

public class CreateUserValidator : AbstractValidator<CreateUser>
{
    public CreateUserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty()
            .Length(Domain.Users.User.MinLengthUsername, Domain.Users.User.MaxLengthUsername)
            .Matches(Domain.Users.User.RegexUsername);
    }
}
