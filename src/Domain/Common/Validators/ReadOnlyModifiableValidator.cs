using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class ReadOnlyModifiableValidator : AbstractValidator<IReadOnlyModifiable>
{
    public ReadOnlyModifiableValidator()
    {
        RuleFor(x => x.ModifiedByName)
            .NotEmpty()
            .MaximumLength(ModifiableEntity.MaxLengthModifiedByName);
    }
}
