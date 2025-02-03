using Desnz.Chmm.Common.Validators;
using Desnz.Chmm.McsSynchronisation.Common.Commands;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Api.Validators;

public class InstallationRequestCommandValidator : AbstractValidator<InstallationRequestCommand>
{
    public InstallationRequestCommandValidator()
    {
        Include(new InstallationPeriodValidator<ActionResult>());
    }
}
