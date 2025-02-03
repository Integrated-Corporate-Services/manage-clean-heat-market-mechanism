using FluentValidation;
using Desnz.Chmm.Identity.Common.Commands.EditOrganisation;

namespace Desnz.Chmm.Identity.Api.Validators.EditOrganisation;

public class EditOrganisationSeniorResponsibleOfficerCommandValidator : AbstractValidator<EditOrganisationSeniorResponsibleOfficerCommand>
{
    public EditOrganisationSeniorResponsibleOfficerCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();

        RuleFor(u => u.Name)
            .NotEmpty().WithMessage("Name is a required field")
            .MaximumLength(100).WithMessage("The Name field has a maximum length of 100 characters")
            .When(c => c.IsApplicantSeniorResponsibleOfficer);

        RuleFor(u => u.JobTitle)
            .MaximumLength(100).WithMessage("The JobTitle field has a maximum length of 100 characters")
            .When(x => !string.IsNullOrEmpty(x.JobTitle))
            .When(c => c.IsApplicantSeniorResponsibleOfficer);

        RuleFor(u => u.TelephoneNumber)
            .NotEmpty().WithMessage("TelephoneNumber is a required field")
            .MaximumLength(100).WithMessage("The TelephoneNumber field has a maximum length of 100 characters")
            .Matches("^[0-9]*$").WithMessage("The TelephoneNumber field should be numeric")
            .When(c => c.IsApplicantSeniorResponsibleOfficer);
    }
}
