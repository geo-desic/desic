using FluentValidation;

namespace Desic.Domain.Users;

public class ReadOnlyUserValidator : AbstractValidator<IReadOnlyUser>
{
    public ReadOnlyUserValidator()
    {
        RuleFor(u => u.Username)
            .NotEmpty()
            .Length(User.MinLengthUsername, User.MaxLengthUsername)
            .Matches(User.RegexUsername);
    }
}
