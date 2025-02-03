using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;
using static Desnz.Chmm.Identity.Api.Constants.OrganisationAddressConstants;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationLegalCorrespondenceAddressCommandValidator : AbstractValidator<EditOrganisationLegalCorrespondenceAddressCommand>
{
    public EditOrganisationLegalCorrespondenceAddressCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();

        RuleFor(u => u.LegalAddressType)
            .NotEmpty().WithMessage("LegalAddressType is a required field")
            .Must(c => LegalCorrespondenceAddressType.AllTypes.Contains(c));

        RuleFor(command => command.LineOne)
            .NotEmpty().WithMessage("LineOne is a required field")
            .MaximumLength(100).WithMessage("The LineOne field has a maximum length of 100 characters")
            .When(c => c.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress);

        RuleFor(command => command.LineTwo)
            .MaximumLength(100).WithMessage("The LineTwo field has a maximum length of 100 characters")
            .When(c => c.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress);

        RuleFor(command => command.City)
            .NotEmpty().WithMessage("City is a required field")
            .MaximumLength(100).WithMessage("The City field has a maximum length of 100 characters")
            .When(c => c.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress);

        RuleFor(command => command.County)
            .MaximumLength(100).WithMessage("The County field has a maximum length of 100 characters")
            .When(c => c.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress);

        RuleFor(command => command.Postcode)
            .NotEmpty().WithMessage("Postcode is a required field")
            .MaximumLength(100).WithMessage("The Postcode field has a maximum length of 100 characters")
            .When(c => c.LegalAddressType == LegalCorrespondenceAddressType.UseSpecifiedAddress);
    }
}
