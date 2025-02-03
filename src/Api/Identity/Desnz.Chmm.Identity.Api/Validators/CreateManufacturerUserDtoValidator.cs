using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class CreateManufacturerUserDtoValidator : AbstractValidator<CreateManufacturerUserDto>
    {
        public CreateManufacturerUserDtoValidator(int? count = null)
        {
            RuleFor(u => u.Name)
                .NotEmpty().WithMessage("Name is a required field")
                .MaximumLength(100).WithMessage("The Name field has a maximum length of 100 characters");

            RuleFor(u => u.Email)
                .NotEmpty().WithMessage("Email is a required field")
                .EmailAddress()
                .MaximumLength(100).WithMessage("The Email field has a maximum length of 100 characters");

            RuleFor(u => u.JobTitle)
                .MaximumLength(100).WithMessage("The JobTitle field has a maximum length of 100 characters")
                .When(x => !string.IsNullOrEmpty(x.JobTitle));

            RuleFor(u => u.TelephoneNumber)
                .NotEmpty().WithMessage("TelephoneNumber is a required field")
                .MaximumLength(100).WithMessage("The TelephoneNumber field has a maximum length of 100 characters")
                .Matches("^[0-9]*$").WithMessage("The TelephoneNumber field should be numeric");   

            RuleFor(u => u.IsResponsibleOfficer)
                .NotNull().WithMessage("IsResponsibleOfficer is a required field");

            RuleFor(u => u.ResponsibleOfficerOrganisationName)
                .NotEmpty().WithMessage("ResponsibleOfficerOrganisationName is a required field")
                .MaximumLength(100).WithMessage("The ResponsibleOfficerOrganisationName field has a maximum length of 100 characters")
                .When(r => r.IsResponsibleOfficer && count > 1);
        }
    }
}
