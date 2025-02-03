using Desnz.Chmm.Common.ValueObjects;
using FluentValidation;

namespace Desnz.Chmm.Common.Validators;

public class InstallationPeriodValidator<TResponse> : AbstractValidator<InstallationPeriod<TResponse>>
{
    public InstallationPeriodValidator()
    {
        RuleFor(c => c.StartDate).NotEmpty();
        RuleFor(c => c.EndDate).NotEmpty().GreaterThanOrEqualTo(y => y.StartDate).WithMessage($"The EndDate must be greater than or equal to StartDate");
    }
}
