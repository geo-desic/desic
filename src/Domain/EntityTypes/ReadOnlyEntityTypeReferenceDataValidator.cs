using FluentValidation;

namespace Desic.Domain.EntityTypes;

public class ReadOnlyEntityTypeReferenceDataValidator : AbstractValidator<IReadOnlyEntityTypeReferenceData>
{
    public ReadOnlyEntityTypeReferenceDataValidator()
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
