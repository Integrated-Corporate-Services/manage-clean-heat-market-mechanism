using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Queries.Quarterly;

public class GetQuarterlyBoilerSalesQueryValidator : AbstractValidator<GetQuarterlyBoilerSalesQuery>
{
    public GetQuarterlyBoilerSalesQueryValidator()
    {
        RuleFor(c => c.SchemeYearId).NotEmpty();
    }
}
