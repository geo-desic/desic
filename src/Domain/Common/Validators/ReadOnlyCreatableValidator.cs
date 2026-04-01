using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class ReadOnlyCreatableValidator : AbstractValidator<IReadOnlyCreatable>
{
    public ReadOnlyCreatableValidator()
    {
        RuleFor(x => x.CreatedByName)
            .NotEmpty()
            .MaximumLength(CreatableEntity.MaxLengthCreatedByName);
    }
}
