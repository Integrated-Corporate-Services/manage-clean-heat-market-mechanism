using Desnz.Chmm.Notes.Api.Extensions;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;
using Desnz.Chmm.Notes.Common.Commands;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Common.Handlers;

namespace Desnz.Chmm.Notes.Api.Handlers.Commands;

public class DeleteManufacturerNoteFileCommandHandler : BaseRequestHandler<DeleteManufacturerNoteFileCommand, ActionResult>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IFileService _fileService;
    private readonly IRequestValidator _requestValidator;

    public DeleteManufacturerNoteFileCommandHandler(
        ILogger<BaseRequestHandler<DeleteManufacturerNoteFileCommand, ActionResult>> logger,
        ICurrentUserService currentUserService,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _currentUserService = currentUserService;
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(DeleteManufacturerNoteFileCommand command, CancellationToken cancellationToken)
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

        var deleteResponse = await _fileService.DeleteObjectNonVersionedBucketAsync(Buckets.Note,
            command.FileName.BuildObjectKeyForFileForNewNote(organisationId, schemeYearId, userId.Value));
        if (deleteResponse.ValidationError != null)
            return ErrorDeletingFile(deleteResponse.ValidationError);

        return Responses.NoContent();
    }
}
