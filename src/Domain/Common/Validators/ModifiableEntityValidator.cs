using Desic.Domain.Common.Entities;
using FluentValidation;

namespace Desic.Domain.Common.Validators;

public class ModifiableEntityValidator : AbstractValidator<ModifiableEntity>
{
    public ModifiableEntityValidator()
    {
        Include(new CreatableEntityValidator());
        Include(new ModifiableValidator());
    }
}
