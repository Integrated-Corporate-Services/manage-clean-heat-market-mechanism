using Desnz.Chmm.ApiClients.Services;
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

public class ClearManufacturerNoteFilesCommandHandler : BaseRequestHandler<ClearManufacturerNoteFilesCommand, ActionResult>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRequestValidator _requestValidator;
    private readonly IFileService _fileService;

    public ClearManufacturerNoteFilesCommandHandler(
        ILogger<BaseRequestHandler<ClearManufacturerNoteFilesCommand, ActionResult>> logger,
        IFileService fileService,
        ICurrentUserService currentUserService,
        IRequestValidator requestValidator) : base(logger)
    {
        _fileService = fileService;
        _currentUserService = currentUserService;
        _requestValidator = requestValidator;
    }

    public override async Task<ActionResult> Handle(ClearManufacturerNoteFilesCommand command, CancellationToken cancellationToken)
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

        var sourceBucketPrefix = NotesFileExtensions.GetObjectKeyPrefixForNewNote(command.OrganisationId, command.SchemeYearId, userId.Value);
        var files = await _fileService.GetFileFullPathsAsync(Buckets.Note, sourceBucketPrefix);
        foreach (var fileKey in files)
        {
            var deleteObjectResult = await _fileService.DeleteObjectNonVersionedBucketAsync(Buckets.Note, fileKey);
            if (!string.IsNullOrWhiteSpace(deleteObjectResult.ValidationError))
            {
                return ExceptionDeletingS3Files(deleteObjectResult.ValidationError);
            }
        }

        return Responses.NoContent();
    }
}
