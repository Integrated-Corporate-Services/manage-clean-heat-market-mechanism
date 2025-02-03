using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Queries;

public class GetManufacturerNoteFileNamesQuery : IRequest<ActionResult<List<string>>>
{
    /// <summary>
    /// Get the manufacturer note file names for the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to retrieve data for</param>
    /// <param name="schemeYearId">Scheme year to retrieve</param>
    /// <param name="noteId">Id for note to retrieve data for (if not new)</param>
    public GetManufacturerNoteFileNamesQuery(Guid? organisationId, Guid? schemeYearId, Guid? noteId)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        NoteId = noteId;
    }

    public Guid? OrganisationId { get; private set; }
    public Guid? SchemeYearId { get; private set; }
    public Guid? NoteId { get; private set; }
}
