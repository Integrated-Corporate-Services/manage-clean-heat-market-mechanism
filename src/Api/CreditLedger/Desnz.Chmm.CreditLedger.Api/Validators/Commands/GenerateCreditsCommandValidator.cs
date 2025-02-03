using Desnz.Chmm.CreditLedger.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.CreditLedger.Api.Validators.Commands;

public class GenerateCreditsCommandValidator : AbstractValidator<GenerateCreditsCommand>
{
    public GenerateCreditsCommandValidator()
    {
        RuleFor(c => c.Installations).NotEmpty();
        RuleForEach(client => client.Installations)
            .SetValidator(new CreditCalculationDtoValidator());
    }
}
