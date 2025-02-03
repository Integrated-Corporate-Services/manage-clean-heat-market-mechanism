using Desnz.Chmm.Identity.Common.Dtos.ManufacturerUser;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class EditManufacturerUserDtoValidator : AbstractValidator<EditManufacturerUserDto>
    {
        public EditManufacturerUserDtoValidator(int? count = null)
        {
            RuleFor(u => u.Id).NotEmpty();
            RuleFor(u => u.Name).NotEmpty().MaximumLength(100);
            RuleFor(u => u.Email).NotEmpty().EmailAddress().MaximumLength(100);
            RuleFor(u => u.JobTitle).NotEmpty().MaximumLength(100);
            RuleFor(u => u.TelephoneNumber).NotEmpty().MaximumLength(100).Matches("^[0-9]*$");
            RuleFor(u => u.IsResponsibleOfficer).NotNull();
            RuleFor(u => u.ResponsibleOfficerOrganisationName).NotEmpty().MaximumLength(100).When(r => r.IsResponsibleOfficer && count > 1);
        }
    }
}
