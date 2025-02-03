using Desnz.Chmm.BoilerSales.Common.Commands.Quarterly.SupportingEvidence;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Quarterly.SupportingEvidence;

public class DeleteQuarterlySupportingEvidenceCommandValidator : AbstractValidator<DeleteQuarterlySupportingEvidenceCommand>
{
    public DeleteQuarterlySupportingEvidenceCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.SchemeYearQuarterId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty();
    }
}
