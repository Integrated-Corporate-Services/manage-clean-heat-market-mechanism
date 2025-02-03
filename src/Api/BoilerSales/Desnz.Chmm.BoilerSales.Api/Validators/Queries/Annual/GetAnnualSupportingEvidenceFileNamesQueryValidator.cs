using Desnz.Chmm.BoilerSales.Common.Queries.Annual.SupportingEvidence;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Queries.Annual;

public class GetAnnualSupportingEvidenceFileNamesQueryValidator : AbstractValidator<GetAnnualSupportingEvidenceFileNamesQuery>
{
    public GetAnnualSupportingEvidenceFileNamesQueryValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
    }
}
