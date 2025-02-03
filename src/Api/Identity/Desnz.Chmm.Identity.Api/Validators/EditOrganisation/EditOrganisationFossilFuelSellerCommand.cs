using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationFossilFuelSellerCommandValidator : AbstractValidator<EditOrganisationFossilFuelSellerCommand>
{
    public EditOrganisationFossilFuelSellerCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
    }
}
