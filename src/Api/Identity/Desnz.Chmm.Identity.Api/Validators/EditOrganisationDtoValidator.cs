using FluentValidation;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;

namespace Desnz.Chmm.Identity.Api.Validators
{

    public class EditOrganisationDtoValidator : AbstractValidator<EditOrganisationDto>
    {
        public EditOrganisationDtoValidator()
        {
            RuleFor(o => o.Id).NotEmpty();
            RuleFor(o => o.IsOnBehalfOfGroup).NotNull();
            RuleFor(o => o.ResponsibleUndertaking).NotEmpty().SetValidator(new ResponsibleUndertakingDtoValidator());
            RuleFor(o => o.Addresses).NotEmpty().Must(a => a.Count >= 1);
            RuleForEach(o => o.Addresses).NotEmpty().SetValidator(new OrganisationAddressDtoValidator());
            RuleFor(o => o.IsFossilFuelBoilerSeller).NotNull();
            RuleFor(o => o.Users).NotEmpty();
            RuleForEach(o => o.Users).NotEmpty().SetValidator(context => new EditManufacturerUserDtoValidator(context.Users.Count));
            RuleFor(o => o.CreditContactDetails).NotEmpty().SetValidator(new CreditContactDetailsDtoValidator());
        }
    }
}
