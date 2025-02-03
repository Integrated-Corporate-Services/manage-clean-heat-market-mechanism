using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly.SupportingEvidence;

public class UploadQuarterlySupportingEvidenceCommandValidator : AbstractValidator<UploadQuarterlySupportingEvidenceCommand>
{
    public UploadQuarterlySupportingEvidenceCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.SchemeYearQuarterId).NotEmpty();

        RuleFor(c => c.SupportingEvidence).NotEmpty();
        RuleForEach(o => o.SupportingEvidence).NotEmpty();
    }
}
