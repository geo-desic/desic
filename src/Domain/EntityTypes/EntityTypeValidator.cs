using FluentValidation;

namespace Desic.Domain.EntityTypes;

public class EntityTypeValidator : AbstractValidator<EntityType>
{
    public EntityTypeValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .Length(SystemEntityTypeValidator.KeyLength)
            .Matches(SystemEntityTypeValidator.KeyRegex);
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(SystemEntityTypeValidator.NameLengthMin, SystemEntityTypeValidator.NameLengthMax)
            .Matches(SystemEntityTypeValidator.NameRegex);
    }
}
