using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationDetailsCommandValidator : AbstractValidator<EditOrganisationDetailsCommand>
{
    public EditOrganisationDetailsCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is a required field")
            .MaximumLength(100).WithMessage("The Name field has a maximum length of 100 characters");

        RuleFor(command => command.CompaniesHouseNumber)
           .MaximumLength(100).WithMessage("Maximum field length of 100 characters exceeded")
           .When(command => !string.IsNullOrEmpty(command.CompaniesHouseNumber));
    }
}
