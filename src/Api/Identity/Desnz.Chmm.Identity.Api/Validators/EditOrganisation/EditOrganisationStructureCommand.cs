using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationStructureCommandValidator : AbstractValidator<EditOrganisationStructureCommand>
{
    public EditOrganisationStructureCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
    }
}
