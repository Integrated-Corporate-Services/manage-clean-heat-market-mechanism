using Desnz.Chmm.BoilerSales.Common.Commands.Annual;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual;

public class SubmitAnnualBoilerSalesCommandValidator : AbstractValidator<SubmitAnnualBoilerSalesCommand>
{
    public SubmitAnnualBoilerSalesCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.Gas).GreaterThanOrEqualTo(0);
        RuleFor(c => c.Oil).GreaterThanOrEqualTo(0);
    }
}
