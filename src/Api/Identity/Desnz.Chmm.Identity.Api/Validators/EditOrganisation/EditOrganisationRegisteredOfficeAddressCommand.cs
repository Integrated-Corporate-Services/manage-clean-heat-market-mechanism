using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationRegisteredOfficeAddressCommandValidator : AbstractValidator<EditOrganisationRegisteredOfficeAddressCommand>
{
    public EditOrganisationRegisteredOfficeAddressCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();

        RuleFor(command => command.LineOne)
            .NotEmpty().WithMessage("LineOne is a required field")
            .MaximumLength(100).WithMessage("The LineOne field has a maximum length of 100 characters");

        RuleFor(command => command.LineTwo)
            .MaximumLength(100).WithMessage("The LineTwo field has a maximum length of 100 characters");

        RuleFor(command => command.City)
            .NotEmpty().WithMessage("City is a required field")
            .MaximumLength(100).WithMessage("The City field has a maximum length of 100 characters");

        RuleFor(command => command.County)
            .MaximumLength(100).WithMessage("The County field has a maximum length of 100 characters");

        RuleFor(command => command.Postcode)
            .NotEmpty().WithMessage("Postcode is a required field")
            .MaximumLength(100).WithMessage("The Postcode field has a maximum length of 100 characters");
    }
}
