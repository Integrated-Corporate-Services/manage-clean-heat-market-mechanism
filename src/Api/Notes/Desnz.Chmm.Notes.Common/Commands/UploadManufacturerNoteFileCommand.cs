using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Desnz.Chmm.Notes.Common.Commands;

public class UploadManufacturerNoteFileCommand : IRequest<ActionResult>
{
    /// <summary>
    /// Upload manufacturer note files to the given manufacturer
    /// </summary>
    /// <param name="organisationId">Manufacturer to upload data for</param>
    /// <param name="schemeYearId">Scheme year to upload for</param>
    /// <param name="files">Files to upload</param>
    public UploadManufacturerNoteFileCommand(Guid organisationId, Guid schemeYearId, List<IFormFile> files)
    {
        OrganisationId = organisationId;
        SchemeYearId = schemeYearId;
        Files = files;
    }

    public List<IFormFile> Files { get; private set; }
    public Guid OrganisationId { get; private set; }
    public Guid SchemeYearId { get; private set; }
}