using Desnz.Chmm.Common.Constants;
using Desnz.Chmm.Obligation.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Obligation.Api.Validators.Commands
{
    public class CreateAnnualObligationCommandValidator : AbstractValidator<CreateAnnualObligationCommand>
    {
        public CreateAnnualObligationCommandValidator()
        {
            RuleFor(o => o.OrganisationId).NotEmpty();
            RuleFor(o => o.SchemeYearId).NotEmpty();
            RuleFor(o => o.TransactionDate).NotEmpty().GreaterThan(DateTime.MinValue);

            Include(new SalesNumbersCommandValidator());
        }
    }
}