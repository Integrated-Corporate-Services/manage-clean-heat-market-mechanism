using Desnz.Chmm.Configuration.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Configuration.Api.Validators;

public class UpdateSchemeYearConfigurationCommandValidator : AbstractValidator<UpdateSchemeYearConfigurationCommand>
{
    public UpdateSchemeYearConfigurationCommandValidator()
    {
        RuleFor(c => c.TargetRate).GreaterThanOrEqualTo(0);
        RuleFor(c => c.PercentageCap).GreaterThanOrEqualTo(0);
        RuleFor(c => c.TargetMultiplier).GreaterThanOrEqualTo(0);
        RuleFor(c => c.CreditCarryOverPercentage).GreaterThanOrEqualTo(0);
        RuleFor(c => c.GasBoilerSalesThreshold).GreaterThanOrEqualTo(0);
        RuleFor(c => c.OilBoilerSalesThreshold).GreaterThanOrEqualTo(0);
    }
}
