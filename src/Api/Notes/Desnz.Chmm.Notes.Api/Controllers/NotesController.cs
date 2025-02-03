using Desnz.Chmm.Notes.Common.Commands;
using Desnz.Chmm.Notes.Common.Dtos;
using Desnz.Chmm.Notes.Common.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Desnz.Chmm.Common.Constants.IdentityConstants;

namespace Desnz.Chmm.Notes.Api.Controllers;

/// <summary>
/// Notes API
/// </summary>
[ApiController]
[Route("api/notes")]
[Authorize(Roles = Roles.Admins)]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;

    /// <summary>
    /// Constructor taking all dependencies
    /// </summary>
    /// <param name="mediator">The mediator for handling commands and queries</param>
    public NotesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// GetAll notes for an organisation
    /// </summary>
    /// <param name="organisationId">Id of organisation to get notes for</param>
    /// <response code="200">Successfully retrieved organisation notes</response>
    [HttpGet("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/notes")]
    public async Task<ActionResult<List<ManufacturerNoteDto>>> GetAllManufacturerNotes(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        return await _mediator.Send(new GetManufacturerNotesQuery(organisationId, schemeYearId), cancellationToken);
    }

    /// <summary>
    /// Creates a note for a Manufacturer
    /// </summary>
    /// <param name="command">Command describing the note to create</param>
    /// <response code="201">Successfully created organisation note</response>
    [HttpPost("manufacturer/note")]
    public async Task<ActionResult<Guid>> CreateManufacturerNote([FromBody] AddManufacturerNoteCommand command, CancellationToken cancellationToken)
    {
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// GetAll file names for existing note
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <param name="noteId">Note id</param>
    /// <response code="200">Successfully retrieved file names</response>
    [HttpGet("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/note/{noteId:guid}/file-names")]
    public async Task<ActionResult<List<string>>> GetExistingNoteFileNames(Guid organisationId, Guid schemeYearId, Guid noteId, CancellationToken cancellationToken)
    {
        var query = new GetManufacturerNoteFileNamesQuery(organisationId, schemeYearId, noteId);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// GetAll file names for new note
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <response code="200">Successfully retrieved file names</response>
    [HttpGet("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/new-note/file-names")]
    public async Task<ActionResult<List<string>>> GetNewNoteFileNames(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        var query = new GetManufacturerNoteFileNamesQuery(organisationId, schemeYearId, null);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Clear files for new note
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <response code="204">Successfully cleared new note files</response>
    [HttpPost("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/new-note/clear-files")]
    public async Task<ActionResult> ClearNewNoteFiles(Guid organisationId, Guid schemeYearId, CancellationToken cancellationToken)
    {
        var command = new ClearManufacturerNoteFilesCommand(organisationId, schemeYearId);
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Upload new file for manufacturer note
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <param name="files">Files to upload</param>
    /// <response code="204">Successfully uploaded file</response>
    [HttpPost("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/new-note/file")]
    public async Task<ActionResult<Guid>> UploadNewNoteFile(Guid organisationId, Guid schemeYearId, [FromForm] List<IFormFile> files, CancellationToken cancellationToken)
    {
        var command = new UploadManufacturerNoteFileCommand(organisationId, schemeYearId, files);
        return await _mediator.Send(command, cancellationToken);
    }

    /// <summary>
    /// Download file for existing manufacturer note
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <param name="noteId">Note id</param>
    /// <param name="fileName">File to download</param>
    /// <response code="200">Successfully downloaded file</response>
    [HttpGet("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/note/{noteId:guid}/download")]
    public async Task<ActionResult<Stream>> DownloadExistingNoteFile([FromRoute] Guid organisationId, [FromRoute] Guid schemeYearId, [FromRoute] Guid noteId, [FromQuery] string fileName, CancellationToken cancellationToken)
    {
        var query = new DownloadManufacturerNoteFileQuery(organisationId, schemeYearId, noteId, fileName);
        return await _mediator.Send(query, cancellationToken);
    }

    /// <summary>
    /// Delete file for new manufacturer note
    /// </summary>
    /// <param name="organisationId">Organisation id</param>
    /// <param name="schemeYearId">Scheme year id</param>
    /// <param name="body">Request body</param>
    /// <response code="204">Successfully deleted file</response>
    [HttpPost("manufacturer/{organisationId:guid}/year/{schemeYearId:guid}/new-note/file/delete")]
    public async Task<ActionResult<Guid>> DeleteNewNoteFile(Guid organisationId, Guid schemeYearId, [FromBody] DeleteManufacturerNoteFileDto body, CancellationToken cancellationToken)
    {
        var command = new DeleteManufacturerNoteFileCommand(organisationId, schemeYearId, body.FileName);
        return await _mediator.Send(command, cancellationToken);
    }
}