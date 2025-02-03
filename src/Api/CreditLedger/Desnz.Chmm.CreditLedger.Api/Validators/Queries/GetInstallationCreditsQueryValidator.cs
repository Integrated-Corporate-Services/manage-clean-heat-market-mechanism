using Desnz.Chmm.Common.Validators;
using Desnz.Chmm.CreditLedger.Common.Dtos;
using Desnz.Chmm.CreditLedger.Common.Queries;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.CreditLedger.Api.Validators.Queries
{
    public class GetInstallationCreditsQueryValidator : AbstractValidator<GetInstallationCreditsQuery>
    {
        public GetInstallationCreditsQueryValidator()
        {
            Include(new InstallationPeriodValidator<ActionResult<List<HeatPumpInstallationCreditsDto>>>());
        }
    }
}
    