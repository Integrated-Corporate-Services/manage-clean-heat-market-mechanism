using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.SupportingEvidence;

public class UploadAnnualSupportingEvidenceCommandValidator : AbstractValidator<UploadAnnualSupportingEvidenceCommand>
{
    public UploadAnnualSupportingEvidenceCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();

        RuleFor(c => c.SupportingEvidence).NotEmpty();
        RuleForEach(o => o.SupportingEvidence).NotEmpty();
    }
}
