using Desnz.Chmm.Identity.Common.Commands;
using Desnz.Chmm.Identity.Common.Dtos.Organisation;
using FluentValidation;
using Newtonsoft.Json;

namespace Desnz.Chmm.Identity.Api.Validators
{
    public class OnboardManufacturerCommandValidator : AbstractValidator<OnboardManufacturerCommand>
    {
        public OnboardManufacturerCommandValidator()
        {
            RuleFor(command => command.OrganisationDetailsJson)
                .Custom((field, context) =>
                {
                    if (string.IsNullOrEmpty(field))
                    {
                        context.AddFailure("OrganisationDetailsJson is a required field");
                        return;
                    }

                    CreateOrganisationDto organisationDetails = null;
                    try
                    {
                        organisationDetails = JsonConvert.DeserializeObject<CreateOrganisationDto>(field);
                    }
                    catch (Exception ex)
                    {
                        context.AddFailure("OrganisationDetailsJson is a required field");
                        return;
                    }

                    var validator = new CreateOrganisationDtoValidator();
                    var result = validator.Validate(organisationDetails);

                    foreach (var error in result.Errors)
                    {
                        context.AddFailure(error);
                    }
                });
        }
    }
}
