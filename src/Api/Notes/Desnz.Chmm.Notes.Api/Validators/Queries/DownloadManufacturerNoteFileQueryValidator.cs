using Desnz.Chmm.Notes.Common.Queries;
using FluentValidation;

namespace Desnz.Chmm.Notes.Api.Validators.Queries;

public class DownloadManufacturerNoteFileQueryValidator : AbstractValidator<DownloadManufacturerNoteFileQuery>
{
    public DownloadManufacturerNoteFileQueryValidator()
    {
        RuleFor(c => c.OrganisationId).NotEmpty();
        RuleFor(c => c.SchemeYearId).NotEmpty();
        RuleFor(c => c.NoteId).NotEmpty();
        RuleFor(c => c.FileName).NotEmpty();
    }
}
