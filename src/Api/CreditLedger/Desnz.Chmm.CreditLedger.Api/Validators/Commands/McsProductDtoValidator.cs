using Desnz.Chmm.McsSynchronisation.Common.Dtos;
using FluentValidation;

namespace Desnz.Chmm.CreditLedger.Api.Validators.Commands;

public class McsProductDtoValidator : AbstractValidator<McsProductDto>
{
    public McsProductDtoValidator()
    {
        RuleFor(c => c.Id).GreaterThan(0);
        RuleFor(c => c.ManufacturerId).GreaterThan(0);
        RuleFor(c => c.Code).NotEmpty();
        RuleFor(c => c.ManufacturerName).NotEmpty();
        RuleFor(c => c.Name).NotEmpty();
    }
}
