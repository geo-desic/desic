using Desic.Domain.Common.Entities;
using Desic.Domain.Common.Interfaces;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class ReadOnlySoftDeletableValidator : AbstractValidator<IReadOnlySoftDeletable>
{
    public ReadOnlySoftDeletableValidator()
    {
        RuleFor(x => x.DeletedByName)
            .MaximumLength(SoftDeletableEntity.MaxLengthDeletedByName);
    }
}
