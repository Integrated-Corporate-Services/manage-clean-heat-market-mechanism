using Desnz.Chmm.BoilerSales.Common.Queries.Annual;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Queries.Annual;

public class GetAnnualBoilerSalesQueryValidator : AbstractValidator<GetAnnualBoilerSalesQuery>
{
    public GetAnnualBoilerSalesQueryValidator()
    {
        RuleFor(c => c.SchemeYearId).NotEmpty();
    }
}
