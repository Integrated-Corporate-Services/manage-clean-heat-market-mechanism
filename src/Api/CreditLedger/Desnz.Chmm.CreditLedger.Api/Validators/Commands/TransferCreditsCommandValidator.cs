using Desnz.Chmm.CreditLedger.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.CreditLedger.Api.Validators.Commands;

public class TransferCreditsCommandValidator : AbstractValidator<TransferCreditsCommand>
{
    public TransferCreditsCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.DestinationOrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.Value).GreaterThan(0).Must(x => (x * 2) % 1 == 0);
    }
}
