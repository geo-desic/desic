using Desic.Domain.Common.Validators;
using FluentValidation;

namespace Desic.Domain.Users;

public class UserValidator : AbstractValidator<User>
{
    public UserValidator()
    {
        Include(new SoftDeletableEntityValidator());
        Include(new ReadOnlyUserValidator());
    }
}
