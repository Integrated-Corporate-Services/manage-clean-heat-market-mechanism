using Desnz.Chmm.Notes.Common.Commands;
using FluentValidation;

namespace Desnz.Chmm.Notes.Api.Validators.Commands;

public class AddManufacturerNoteCommandValidator : AbstractValidator<AddManufacturerNoteCommand>
{
    public AddManufacturerNoteCommandValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.Details)
            .NotEmpty()
            .MaximumLength(1200).WithMessage("Note text cannot be more than 1200 characters in length");
    }
}