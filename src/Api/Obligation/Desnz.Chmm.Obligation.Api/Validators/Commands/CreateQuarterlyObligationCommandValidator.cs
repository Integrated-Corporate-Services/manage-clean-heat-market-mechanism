using Desnz.Chmm.Obligation.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Obligation.Api.Validators.Commands
{
    public class CreateQuarterlyObligationCommandValidator : AbstractValidator<CreateQuarterlyObligationCommand>
    {
        public CreateQuarterlyObligationCommandValidator()
        {
            RuleFor(o => o.SchemeYearQuarterId).NotEmpty();

            Include(new CreateAnnualObligationCommandValidator());
        }
    }
}