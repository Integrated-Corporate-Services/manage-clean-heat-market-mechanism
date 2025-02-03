using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationHeatPumpSellerCommandValidator : AbstractValidator<EditOrganisationHeatPumpSellerCommand>
{
    public EditOrganisationHeatPumpSellerCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
    }
}
