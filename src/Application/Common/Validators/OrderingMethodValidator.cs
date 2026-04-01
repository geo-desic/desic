using Desic.Application.Common.Infrastructure;
using Desic.Application.Common.Interfaces;
using FluentValidation;

namespace Desic.Application.Common.Validators;

public class OrderingMethodValidator : AbstractValidator<IOrderingMethod>
{
    public OrderingMethodValidator()
    {
        RuleFor(o => o.OrderBy)
            .NotEmpty()
            .Must(x => x.Count <= OrderingMethods.MaximumOrderByCount)
            .Must(x => !x.GroupBy(x => x.Property).Any(x => x.Count() > 1)).WithMessage($"All {nameof(IOrderingMethod.OrderBy)}.{nameof(IOrderBy.Property)} values must be unique");
    }
}
