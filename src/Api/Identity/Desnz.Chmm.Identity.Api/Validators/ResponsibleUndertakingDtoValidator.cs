using Desnz.Chmm.Identity.Common.Dtos;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class ResponsibleUndertakingDtoValidator : AbstractValidator<ResponsibleUndertakingDto>
    {
        public ResponsibleUndertakingDtoValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty().WithMessage("Name is a required field");

            RuleFor(command => command.CompaniesHouseNumber)
               .MaximumLength(100).WithMessage("Maximum field length of 100 characters exceeded")
               .When(command => !string.IsNullOrEmpty(command.CompaniesHouseNumber));
        }
    }
}
