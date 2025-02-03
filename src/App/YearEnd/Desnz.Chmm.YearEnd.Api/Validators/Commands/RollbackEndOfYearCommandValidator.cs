using Desnz.Chmm.YearEnd.Api.Controllers;
using FluentValidation;

namespace Desnz.Chmm.YearEnd.Api.Validators.Commands;

public class RollbackEndOfYearCommandValidator : AbstractValidator<RollbackEndOfYearCommand>
{
    public RollbackEndOfYearCommandValidator()
    {
        RuleFor(c => c.SchemeYearId).NotEmpty();
    }
}
