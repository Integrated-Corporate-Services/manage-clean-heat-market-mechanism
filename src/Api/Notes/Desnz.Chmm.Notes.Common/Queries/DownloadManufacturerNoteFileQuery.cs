using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Queries;

public class DownloadManufacturerNoteFileQuery : IRequest<ActionResult<Stream>>
{
    /// <summary>
    /// Download a manufacturer note file from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to get data for</param>
    /// <param name="schemeYearId">Scheme year to get for</param>
    /// <param name="noteId">Id for note to get data for (if not new)</param>
    /// <param name="fileName">File name to get</param>
    public DownloadManufacturerNoteFileQuery(Guid? organisationId, Guid? schemeYearId, Guid? noteId, string fileName)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        NoteId = noteId;
        FileName = fileName;
    }

    public Guid? OrganisationId { get; private set; }
    public Guid? SchemeYearId { get; private set; }
    public Guid? NoteId { get; private set; }
    public string FileName { get; private set; }
}
