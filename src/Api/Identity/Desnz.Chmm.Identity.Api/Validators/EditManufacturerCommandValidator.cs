using Desnz.Chmm.Identity.Common.Commands;
using FluentValidation;
using Newtonsoft.Json;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class EditManufacturerCommandValidator : AbstractValidator<EditManufacturerCommand>
    {
        public EditManufacturerCommandValidator()
        {
            RuleFor(x => x.OrganisationDetailsJson).Custom((field, context) =>
            {
                if (string.IsNullOrEmpty(field))
                {
                    context.AddFailure("OrganisationDetailsJson is a required field");
                    return;
                }

                EditOrganisationDto? organisationDetails = null;
                try
                {
                    organisationDetails = JsonConvert.DeserializeObject<EditOrganisationDto>(field);
                }
                catch (Exception ex)
                {
                    context.AddFailure("OrganisationDetailsJson is a required field");
                    return;
                }

                var validator = new EditOrganisationDtoValidator();
                var result = validator.Validate(organisationDetails!);

                foreach (var error in result.Errors)
                {
                    context.AddFailure(error);
                }
            });
        }
    }
}
