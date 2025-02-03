using Desnz.Chmm.CreditLedger.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.CreditLedger.Api.Validators.Commands;

public class CarryOverNewLicenceHoldersCommandValidator : AbstractValidator<CarryOverNewLicenceHoldersCommand>
{
    public CarryOverNewLicenceHoldersCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.LicenceHolderId).NotEmpty();
        RuleFor(c => c.PreviousSchemeYearId).NotEmpty();
        RuleFor(c => c.StartDate).NotEmpty().Must(p => !(p == DateOnly.MinValue));
    }
}
