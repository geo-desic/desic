using FluentValidation;

namespace Desic.Domain.EntityTypes;

public class EntityTypeValidator : AbstractValidator<EntityType>
{
    public EntityTypeValidator()
    {
        Include(new ReadOnlyEntityTypeReferenceDataValidator());
    }
}
