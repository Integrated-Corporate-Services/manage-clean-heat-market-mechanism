using Desnz.Chmm.BoilerSales.Common.Queries.Annual.VerificationStatement;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Queries.Annual;

public class GetAnnualVerificationStatementFileNamesQueryValidator : AbstractValidator<GetAnnualVerificationStatementFileNamesQuery>
{
    public GetAnnualVerificationStatementFileNamesQueryValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
    }
}
