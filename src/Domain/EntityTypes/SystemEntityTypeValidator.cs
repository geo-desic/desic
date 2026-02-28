using FluentValidation;

namespace Desic.Domain.EntityTypes;

public class SystemEntityTypeValidator : AbstractValidator<SystemEntityType>
{
    internal const int KeyLength = 4;
    internal const string KeyRegex = "^[a-z]*$"; // lowercase alphabetic characters
    internal const int NameLengthMin = 2;
    internal const int NameLengthMax = 25;
    internal const string NameRegex = "^[A-Z]{1}[a-zA-Z0-9]*$"; // starts with an upper case alphabetic character and contains only alphanumeric characters

    public SystemEntityTypeValidator()
    {
        RuleFor(x => x.Key)
            .NotEmpty()
            .Length(KeyLength)
            .Matches(KeyRegex);
        RuleFor(x => x.Name)
            .NotEmpty()
            .Length(NameLengthMin, NameLengthMax)
            .Matches(NameRegex);
    }
}
