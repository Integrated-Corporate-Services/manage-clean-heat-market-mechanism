using Desnz.Chmm.BoilerSales.Common.Queries.Quarterly.SupportingEvidence;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Queries.Quarterly;

public class GetQuarterlySupportingEvidenceFileNamesQueryValidator : AbstractValidator<GetQuarterlySupportingEvidenceFileNamesQuery>
{
    public GetQuarterlySupportingEvidenceFileNamesQueryValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.SchemeYearQuarterId).NotEmpty();
    }
}
