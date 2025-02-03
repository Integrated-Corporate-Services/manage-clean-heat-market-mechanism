using Desnz.Chmm.Notes.Api.Extensions;
using Desnz.Chmm.Notes.Common.Queries;
using Desnz.Chmm.Common.Services;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;
using Microsoft.AspNetCore.Mvc;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.CommonValidation;

namespace Desnz.Chmm.Notes.Api.Handlers.Queries;

public class DownloadManufacturerNoteFileQueryHandler : BaseRequestHandler<DownloadManufacturerNoteFileQuery, ActionResult<Stream>>
{ 
    private readonly IFileService _fileService;
    private readonly IManufacturerNotesRepository _manufacturerNotesRepository;
    private readonly IRequestValidator _requestValidator;

    public DownloadManufacturerNoteFileQueryHandler(
        ILogger<DownloadManufacturerNoteFileQueryHandler> logger,
        IFileService fileService,
        IManufacturerNotesRepository manufacturerNotesRepository,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _manufacturerNotesRepository = manufacturerNotesRepository;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult<Stream>> Handle(DownloadManufacturerNoteFileQuery query, CancellationToken cancellationToken)
    {
        var organisationId = query.OrganisationId;
        var schemeYearId = query.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        var note = await _manufacturerNotesRepository.GetById(query.NoteId.Value);
        if (note == null)
            return CannotFindNote(query.NoteId.Value);

        var downloadResponse = await _fileService.DownloadFileAsync(Buckets.Note, query.FileName.BuildObjectKeyForFileForExistingNote(query.NoteId.Value));

        switch (downloadResponse.ValidationError)
        {
            case null:
                return Responses.File(downloadResponse.FileContent, downloadResponse.ContentType, query.FileName);
            case "NotFound":
                return (ActionResult<Stream>)ErrorDownloadingFile(string.Format("Could not download a Manufacturer Note File with name: {0} for organisation with Id {1}", query.FileName, query.OrganisationId));
            default:
                return (ActionResult<Stream>)Responses.BadRequest(downloadResponse.ValidationError);
        }
    }
}
