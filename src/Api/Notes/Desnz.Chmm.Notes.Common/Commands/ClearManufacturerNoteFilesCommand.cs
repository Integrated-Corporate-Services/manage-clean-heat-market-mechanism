using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Commands;

public class ClearManufacturerNoteFilesCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Clear manufacturer new note files from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to delete data for</param>
    /// <param name="schemeYearId">Scheme year to delete for</param>
    /// <remarks>
    /// This is only for clearing new note files - it will not clear files for existing notes
    /// </remarks>
    public ClearManufacturerNoteFilesCommand(Guid organisationId, Guid schemeYearId)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
}
