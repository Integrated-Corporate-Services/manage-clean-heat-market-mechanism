using Desnz.Chmm.BoilerSales.Common.Commands.Annual.VerificationStatement;
using FluentValidation;

namespace Desnz.Chmm.BoilerSales.Api.Validators.Commands.Annual.VerificationStatement;

public class UploadAnnualVerificationStatementCommandValidator : AbstractValidator<UploadAnnualVerificationStatementCommand>
{
    public UploadAnnualVerificationStatementCommandValidator()
    {
        RuleFor(c => c.ManufacturerId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();

        RuleFor(c => c.VerificationStatement).NotEmpty();
        RuleForEach(o => o.VerificationStatement).NotEmpty();
    }
}
