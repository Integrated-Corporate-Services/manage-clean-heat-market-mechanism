using Desnz.Chmm.ApiClients.Services;
using Desnz.Chmm.Common.Extensions;
using Desnz.Chmm.Common.Handlers;
using Desnz.Chmm.Common.Mediator;
using Desnz.Chmm.Common.Services;
using Desnz.Chmm.CommonValidation;
using Desnz.Chmm.Notes.Api.Entities;
using Desnz.Chmm.Notes.Api.Extensions;
using Desnz.Chmm.Notes.Api.Infrastructure.Repositories;
using Desnz.Chmm.Notes.Common.Commands;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Notes.Api.Constants.NoteConstants;

namespace Desnz.Chmm.Notes.Api.Handlers.Commands;

/// <summary>
/// Handles the Add Manufacturer Note Command
/// </summary>
public class AddManufacturerNoteCommandHandler : BaseRequestHandler<AddManufacturerNoteCommand, ActionResult<Guid>>
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IManufacturerNotesRepository _manufacturerNotesRepository;
    private readonly IOrganisationService _organisationService;
    private readonly IFileService _fileService;
    private readonly IRequestValidator _requestValidator;

    /// <summary>
    /// Constructor taking all dependencies
    /// </summary>
    /// <param name="logger">The logger</param>
    /// <param name="currentUserService">Context for getting the current user details</param>
    /// <param name="organisationService">The Organisartion service for getting Organisation Details</param>
    /// <param name="manufacturerNotesRepository">The repository for creating the note</param>
    public AddManufacturerNoteCommandHandler(
        ILogger<BaseRequestHandler<AddManufacturerNoteCommand, ActionResult<Guid>>> logger,
        ICurrentUserService currentUserService,
        IManufacturerNotesRepository manufacturerNotesRepository,
        IOrganisationService organisationService,
        IFileService fileService,
        IRequestValidator requestValidator) : base(logger)
    {
        _currentUserService = currentUserService;
        _manufacturerNotesRepository = manufacturerNotesRepository;
        _organisationService = organisationService;
        _fileService = fileService;
        _requestValidator = requestValidator;
    }

    /// <summary>
    /// Handles the add manufacturer command
    /// </summary>
    /// <param name="command">The command to handle</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns></returns>
    /// <exception cref="NotImplementedException"></exception>
    public override async Task<ActionResult<Guid>> Handle(AddManufacturerNoteCommand command, CancellationToken cancellationToken)
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

        var note = new ManufacturerNote(command.OrganisationId, command.SchemeYearId, command.Details, userId.ToString());

        await _manufacturerNotesRepository.Create(note);

        var sourceBucketPrefix = NotesFileExtensions.GetObjectKeyPrefixForNewNote(command.OrganisationId, schemeYearId, userId.Value);
        var files = await _fileService.GetFileFullPathsAsync(Buckets.Note, sourceBucketPrefix);
        foreach (var previousFileKey in files)
        {
            var newFileKey = $"{note.Id}/{previousFileKey.Split('/').Last()}";
            var copyFileResult = await _fileService.CopyFileAsync(Buckets.Note, previousFileKey, Buckets.Note, newFileKey);
            if (!string.IsNullOrWhiteSpace(copyFileResult.ValidationError))
            {
                return ExceptionCopyingS3Files(copyFileResult.ValidationError);
            }
            var deleteObjectResult = await _fileService.DeleteObjectNonVersionedBucketAsync(Buckets.Note, previousFileKey);
            if (!string.IsNullOrWhiteSpace(deleteObjectResult.ValidationError))
            {
                return ExceptionDeletingS3Files(deleteObjectResult.ValidationError);
            }
        }

        return Responses.Created(note.Id);
    }
}
