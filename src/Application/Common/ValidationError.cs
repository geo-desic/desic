using FluentValidation.Results;

namespace Desic.Application.Common;

public class ValidationError() : Error("One or more validation failures occurred.")
{
    public ValidationError(IEnumerable<ValidationFailure> failures) : this()
    {
        Failures = failures.GroupBy(f => f.PropertyName, e => e.ErrorMessage).ToDictionary(g => g.Key, g => g.ToArray());
    }

    public IDictionary<string, string[]> Failures { get; } = new Dictionary<string, string[]>();
}
