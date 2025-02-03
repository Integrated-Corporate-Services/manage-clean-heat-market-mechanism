using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using FluentValidation;

namespace Desnz.Chmm.CreditLedger.Api.Validators.Commands;

public class CreditCalculationDtoValidator : AbstractValidator<McsInstallationDto>
{
    public CreditCalculationDtoValidator()
    {
        RuleFor(c => c.CommissioningDate).NotEmpty().GreaterThan(DateTime.MinValue);
        RuleFor(c => c.MidId).GreaterThan(0);
       RuleForEach(client => client.HeatPumpProducts).SetValidator(new McsProductDtoValidator());
    }
}
