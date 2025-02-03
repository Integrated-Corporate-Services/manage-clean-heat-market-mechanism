using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly;

public class UpdateQuarterlyBoilerSalesCommandValidator : AbstractValidator<EditQuarterlyBoilerSalesCommand>
{
    public UpdateQuarterlyBoilerSalesCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.SchemeYearQuarterId).NotEmpty();
        RuleFor(c => c.Gas).GreaterThanOrEqualTo(0);
        RuleFor(c => c.Oil).GreaterThanOrEqualTo(0);
    }
}
