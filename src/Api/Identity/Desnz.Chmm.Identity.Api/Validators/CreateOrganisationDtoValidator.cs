using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using FluentValidation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class CreateOrganisationDtoValidator : AbstractValidator<CreateOrganisationDto>
    {
        public CreateOrganisationDtoValidator()
        {
            RuleFor(o => o.IsOnBehalfOfGroup).NotNull();
            RuleFor(o => o.ResponsibleUndertaking).NotEmpty().SetValidator(new ResponsibleUndertakingDtoValidator());

            RuleFor(o => o.Addresses).NotEmpty();
            RuleForEach(o => o.Addresses).NotEmpty().SetValidator(new CreateOrganisationAddressDtoValidator());

            RuleFor(o => o.IsFossilFuelBoilerSeller).NotNull();

            RuleFor(o => o.Users).NotEmpty();
            RuleForEach(o => o.Users).NotEmpty().SetValidator(context => new CreateManufacturerUserDtoValidator(context.Users.Count));

            RuleFor(o => o.CreditContactDetails).NotEmpty().SetValidator(new CreditContactDetailsDtoValidator());
        }
    }
}