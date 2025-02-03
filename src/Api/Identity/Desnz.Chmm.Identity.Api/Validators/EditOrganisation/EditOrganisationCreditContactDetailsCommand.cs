using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationCreditContactDetailsCommandValidator : AbstractValidator<EditOrganisationCreditContactDetailsCommand>
{
    public EditOrganisationCreditContactDetailsCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is a required field")
            .MaximumLength(100).WithMessage("The Name field has a maximum length of 100 characters");

        RuleFor(c => c.Email).NotEmpty().WithMessage(c => $"Either {nameof(c.Email)} or {nameof(c.TelephoneNumber)} must not be empty.").When(c => !string.IsNullOrWhiteSpace(c.Name) && string.IsNullOrWhiteSpace(c.TelephoneNumber)).EmailAddress().MaximumLength(100);
        RuleFor(c => c.TelephoneNumber).NotEmpty().WithMessage(c => $"Either {nameof(c.Email)} or {nameof(c.TelephoneNumber)} must not be empty.").When(c => !string.IsNullOrWhiteSpace(c.Name) && string.IsNullOrWhiteSpace(c.Email)).Matches("^[0-9]*$").MaximumLength(100);
    }
}
