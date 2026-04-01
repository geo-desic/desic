using Desic.Domain.Common.Entities;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class CreatableEntityValidator : AbstractValidator<CreatableEntity>
{
    public CreatableEntityValidator()
    {
        Include(new ReadOnlyCreatableValidator());
    }
}
