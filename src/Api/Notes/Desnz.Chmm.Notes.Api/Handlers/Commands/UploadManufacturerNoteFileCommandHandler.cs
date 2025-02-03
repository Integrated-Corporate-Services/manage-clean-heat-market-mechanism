using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Notes.Api.Extensions;
using Desnz.Chmm.Notes.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;

namespace Desnz.Chmm.Notes.Api.Handlers.Commands;

public class UploadManufacturerNoteFileCommandHandler : BaseRequestHandler<UploadManufacturerNoteFileCommand, ActionResult>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestValidator _requestValidator;
    private readonly IFileService _fileService;


    public UploadManufacturerNoteFileCommandHandler(
        ILogger<BaseRequestHandler<UploadManufacturerNoteFileCommand, ActionResult>> logger,
        IFileService fileService,
        ICurrentUserService currentUserService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _currentUserService = currentUserService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(UploadManufacturerNoteFileCommand command, CancellationToken cancellationToken)
    {
        var organisationId = command.OrganisationId;
        var schemeYearId = command.SchemeYearId;

        var validationError = await _requestValidator.ValidateSchemeYearAndOrganisation(
            organisationId: organisationId,
            schemeYearId: schemeYearId);
        if (validationError != null)
            return validationError;

        var user = _currentUserService.CurrentUser;
        var userId = user.GetUserId();

        foreach (var file in command.Files)
        {
            var uploadResponse = await _fileService.UploadFileAsync(Buckets.Note, file.BuildObjectKeyForFileForNewNote(organisationId, schemeYearId, userId.Value), file);
            if (uploadResponse.ValidationError != null)
                return ErrorUploadingFile(uploadResponse.ValidationError);
        }

        return Responses.NoContent();
    }
}
