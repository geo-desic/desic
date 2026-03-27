using FluentValidation;

namespace Desic.Domain.EntityTypes;

public class SystemEntityTypeValidator : AbstractValidator<SystemEntityType>
{
    public SystemEntityTypeValidator()
    {
        Include(new ReadOnlyEntityTypeValidator());
    }
}
