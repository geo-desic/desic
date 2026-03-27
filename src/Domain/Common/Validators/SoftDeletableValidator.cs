using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class SoftDeletableValidator : AbstractValidator<ISoftDeletable>
{
    public SoftDeletableValidator()
    {
        RuleFor(x => x.DeletedByName)
            .MaximumLength(SoftDeletableEntity.MaxLengthDeletedByName);
    }
}
