using Desnz.Chmm.Notes.Common.Queries;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Services;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;
using Desnz.Chmm.Notes.Api.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.CommonValidation;

namespace Desnz.Chmm.Notes.Api.Handlers.Queries;

public class GetManufacturerNoteFileNamesQueryHandler : BaseRequestHandler<GetManufacturerNoteFileNamesQuery, ActionResult<List<string>>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IManufacturerNotesRepository _manufacturerNotesRepository;
    private readonly IRequestValidator _requestValidator;

    public GetManufacturerNoteFileNamesQueryHandler(
        ILogger<GetManufacturerNoteFileNamesQueryHandler> logger,
        ICurrentUserService currentUserService,
        IFileService fileService,
        IManufacturerNotesRepository manufacturerNotesRepository,
        IRequestValidator requestValidator) : base(logger)
    {
        _currentUserService = currentUserService;
        _fileService = fileService;
        _manufacturerNotesRepository = manufacturerNotesRepository;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult<List<string>>> Handle(GetManufacturerNoteFileNamesQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        if (query.NoteId.HasValue)
        {
            var note = await _manufacturerNotesRepository.GetById(query.NoteId.Value);
            if (note == null)
                return CannotFindNote(query.NoteId.Value);

            return await _fileService.GetFileNamesAsync(
                Buckets.Note,
                NotesFileExtensions.GetObjectKeyPrefixForExistingNote(query.NoteId.Value));
        }
        else
        {
            var user = _currentUserService.CurrentUser;
            var userId = user.GetUserId();

            return await _fileService.GetFileNamesAsync(
                Buckets.Note,
                NotesFileExtensions.GetObjectKeyPrefixForNewNote(query.OrganisationId.Value, query.SchemeYearId.Value, userId.Value));
        }
    }
}
