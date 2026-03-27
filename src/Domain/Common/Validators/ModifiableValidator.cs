using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class ModifiableValidator : AbstractValidator<IModifiable>
{
    public ModifiableValidator()
    {
        RuleFor(x => x.ModifiedByName)
            .NotEmpty()
            .MaximumLength(ModifiableEntity.MaxLengthModifiedByName);
    }
}
