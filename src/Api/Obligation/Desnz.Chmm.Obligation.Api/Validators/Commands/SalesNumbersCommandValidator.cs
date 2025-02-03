using Desnz.Chmm.Obligation.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Obligation.Api.Validators.Commands
{
    public class SalesNumbersCommandValidator : AbstractValidator<SalesNumbersCommand>
    {
        public SalesNumbersCommandValidator()
        {
            RuleFor(o => o.Gas).NotEmpty().GreaterThanOrEqualTo(0);
            RuleFor(o => o.Oil).NotEmpty().GreaterThanOrEqualTo(0);
        }
    }
}