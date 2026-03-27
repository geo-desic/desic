using Desic.Domain.Common.Entities;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class SoftDeletableEntityValidator : AbstractValidator<SoftDeletableEntity>
{
    public SoftDeletableEntityValidator()
    {
        Include(new ModifiableEntityValidator());
        Include(new SoftDeletableValidator());
    }
}
