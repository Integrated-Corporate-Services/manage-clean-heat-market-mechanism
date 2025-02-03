using Desnz.Chmm.Obligation.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Obligation.Api.Validators.Commands
{
    public class AmendObligationCommandValidator : AbstractValidator<AmendObligationCommand>
    {
        public AmendObligationCommandValidator()
        {
            RuleFor(o => o.OrganisationId).NotEmpty();
            RuleFor(o => o.SchemeYearId).NotEmpty();
            RuleFor(o => o.Value).NotNull();
        }
    }
}
