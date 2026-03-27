using FluentValidation;

namespace Desic.Domain.EntityTypes;

public class ReadOnlyEntityTypeValidator : AbstractValidator<IReadOnlyEntityType>
{
    public ReadOnlyEntityTypeValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .Length(EntityType.LengthKey)
            .Matches(EntityType.RegexKey);
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(EntityType.MinLengthName, EntityType.MaxLengthName)
            .Matches(EntityType.RegexName);
    }
}
