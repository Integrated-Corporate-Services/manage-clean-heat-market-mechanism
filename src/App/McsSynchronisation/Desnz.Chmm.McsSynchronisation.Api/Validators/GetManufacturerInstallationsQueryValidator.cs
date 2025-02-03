using Desnz.Chmm.Common.Validators;
using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using Desnz.Chmm.McsSynchronisation.Common.Queries;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.McsSynchronisation.Api.Validators;

public class GetManufacturerInstallationsQueryValidator : AbstractValidator<GetManufacturerInstallationsQuery>
{
    public GetManufacturerInstallationsQueryValidator()
    {
        Include(new InstallationPeriodValidator<ActionResult<List<CreditCalculationDto>>>());
    }
}
