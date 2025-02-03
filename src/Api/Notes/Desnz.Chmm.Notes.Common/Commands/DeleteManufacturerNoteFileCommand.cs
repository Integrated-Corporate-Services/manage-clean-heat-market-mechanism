using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Commands;

public class DeleteManufacturerNoteFileCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Delete manufacturer note file from the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to delete data for</param>
    /// <param name="schemeYearId">Scheme year to delete for</param>
    /// <param name="fileName">File name to delete</param>
    public DeleteManufacturerNoteFileCommand(Guid organisationId, Guid schemeYearId, string fileName)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        FileName = fileName;
    }

    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
    public string FileName { get; private set; }
}
