using Desnz.Chmm.BoilerSales.Common.Commands.Annual.SupportingEvidence;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.SupportingEvidence;

public class DeleteAnnualSupportingEvidenceCommandValidator : AbstractValidator<DeleteAnnualSupportingEvidenceCommand>
{
    public DeleteAnnualSupportingEvidenceCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty();
    }
}
