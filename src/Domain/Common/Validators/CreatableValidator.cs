using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class CreatableValidator : AbstractValidator<ICreatable>
{
    public CreatableValidator()
    {
        RuleFor(x => x.CreatedByName)
            .NotEmpty()
            .MaximumLength(CreatableEntity.MaxLengthCreatedByName);
    }
}
