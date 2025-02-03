using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.VerificationStatement;

public class DeleteAnnualVerificationStatementCommandValidator : AbstractValidator<DeleteAnnualVerificationStatementCommand>
{
    public DeleteAnnualVerificationStatementCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty();
    }
}
